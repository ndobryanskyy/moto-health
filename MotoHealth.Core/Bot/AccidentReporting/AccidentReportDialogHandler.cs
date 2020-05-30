using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting.Exceptions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram.Messages;
using Telegram.Bot.Types.ReplyMarkups;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public interface IAccidentReportDialogHandler
    {
        Task<bool> AdvanceDialogAsync(IChatUpdateContext context, IAccidentReportDialogState state, CancellationToken cancellationToken);
    }

    internal sealed class AccidentReportDialogHandler : IAccidentReportDialogHandler
    {
        private readonly ILogger<AccidentReportDialogHandler> _logger;
        private readonly IAccidentReportingService _accidentReportingService;
        private readonly IBotTelemetryService _botTelemetry;

        public AccidentReportDialogHandler(
            ILogger<AccidentReportDialogHandler> logger,
            IAccidentReportingService accidentReportingService, 
            IBotTelemetryService botTelemetry)
        {
            _logger = logger;
            _accidentReportingService = accidentReportingService;
            _botTelemetry = botTelemetry;
        }

        /// <summary>
        /// Advances dialog with specified context and state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="state"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if dialog ended.</returns>
        public async Task<bool> AdvanceDialogAsync(
            IChatUpdateContext context,
            IAccidentReportDialogState state,
            CancellationToken cancellationToken)
        {
            async Task SendMessageAsync(IMessage message)
            {
                await context.SendMessageAsync(message, cancellationToken);
            }

            var update = context.Update;
            var dialogTelemetry = _botTelemetry.GetTelemetryServiceForAccidentReporting(state);

            var cancelled = CheckIfCancelled();
            if (cancelled)
            {
                dialogTelemetry.OnCancelled();

                await SendMessageAsync(Messages.Cancelled);

                return true;
            }

            try
            {
                var dialogCompleted = await HandleStepAsync();

                if (!dialogCompleted)
                {
                    state.CurrentStep++;
                    dialogTelemetry.OnNextStep();
                }
                else
                {
                    dialogTelemetry.OnCompleted();
                }

                return dialogCompleted;
            }
            catch (UnexpectedReplyTypeException)
            {
                dialogTelemetry.OnUnexpectedReply();

                return false;
            }
            catch (ReplyValidationException replyValidationException)
            {
                dialogTelemetry.OnReplyValidationFailed();

                await SendMessageAsync(replyValidationException.UserFriendlyErrorMessage);

                return false;
            }

            async Task<bool> HandleStepAsync()
            {
                switch (state.CurrentStep)
                {
                    case 0:
                        {
                            await SendMessageAsync(Messages.SpecifyAddress);
                            break;
                        }

                    case 1:
                        {
                            if (update is IMapLocation location)
                            {
                                dialogTelemetry.OnLocationSentAutomatically();

                                state.Location = location;
                            }
                            else if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                EnsureMaxLengthNotExceeded(text, 100);
                                state.Address = text;
                            }
                            else
                            {
                                throw new UnexpectedReplyTypeException();
                            }

                            await SendMessageAsync(Messages.SpecifyParticipants);
                            break;
                        }

                    case 2:
                        {
                            if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                EnsureMaxLengthNotExceeded(text, 100);
                                state.Participant = text;

                                await SendMessageAsync(Messages.AreThereVictims);
                                break;
                            }
                            
                            throw new UnexpectedReplyTypeException();
                        }

                    case 3:
                        {
                            if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                EnsureMaxLengthNotExceeded(text, 100);
                                state.Victims = text;

                                await SendMessageAsync(Messages.AskForContacts);
                                break;
                            }

                            throw new UnexpectedReplyTypeException();
                        }

                    case 4:
                        {
                            if (update is IContactMessageBotUpdate { Contact: { PhoneNumber: var phoneNumber } })
                            {
                                dialogTelemetry.OnPhoneNumberSharedAutomatically();

                                state.ReporterPhoneNumber = phoneNumber;
                            }
                            else if (update is ITextMessageBotUpdate { Text: var text })
                            {
                                EnsureMaxLengthNotExceeded(text, 50);

                                state.ReporterPhoneNumber = text.Trim().Equals(Messages.SkipButton.Text, StringComparison.InvariantCultureIgnoreCase) 
                                    ? "Не указан"
                                    : text;
                            }
                            else
                            {
                                throw new UnexpectedReplyTypeException();
                            }

                            await SendMessageAsync(Messages.ReportSummary(state));
                            break;
                        }

                    case 5:
                        {
                            if (update is ITextMessageBotUpdate textMessage)
                            {
                                if (textMessage.Text.Trim().Equals(Messages.SubmitButton.Text, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    await ReportAccidentAsync();
                                    await SendMessageAsync(Messages.SuccessfullySent);

                                    return true;
                                }
                                else
                                {
                                    throw new ReplyValidationException(Messages.ConfirmationExpected);
                                }
                            }

                            throw new UnexpectedReplyTypeException();
                        }


                    default:
                        _logger.LogError("Got unexpected step number");
                        return true;
                }

                return false;
            } 

            bool CheckIfCancelled() => update is ITextMessageBotUpdate { Text: var text } &&
                                       text.Trim().Equals(Messages.CancelButton.Text, StringComparison.InvariantCultureIgnoreCase);

            async Task ReportAccidentAsync()
            {
                var report = AccidentReport.CreateFromDialogState(state);

                report.ReporterTelegramUserId = update.Sender.Id;
                report.ReportedAtUtc = DateTime.UtcNow;

                await _accidentReportingService.ReportAccidentAsync(report, cancellationToken);
            }
        }

        private static void EnsureMaxLengthNotExceeded(string text, int maxLength)
        {
            if (text.Length > maxLength)
            {
                throw new ReplyValidationException(Messages.ReplyMaxLengthExceeded(maxLength));
            }
        }

        private static class Messages
        {
            public static readonly KeyboardButton CancelButton = new KeyboardButton("Отмена");
            public static readonly KeyboardButton ShareNumber = KeyboardButton.WithRequestContact("Мой номер");
            public static readonly KeyboardButton ShareCurrentLocation = KeyboardButton.WithRequestLocation("Мое местоположение");
            public static readonly KeyboardButton SkipButton = new KeyboardButton("Пропустить");
            public static readonly KeyboardButton SubmitButton = new KeyboardButton("Отправить");

            public static readonly IMessage Cancelled = MessageFactory.CreateTextMessage()
                .WithPlainText("⛔ Отменено")
                .WithClearedReplyKeyboard();

            private static readonly IMessage SpecifyAddressPrompt = MessageFactory.CreateTextMessage()
                .WithPlainText("📍 Адрес ДТП");

            private static readonly IMessage SpecifyAddressHint = MessageFactory.CreateTextMessage()
                .WithMarkdownText($"Нажмите *{ShareCurrentLocation.Text}*, чтобы автоматически отправить место на карте, где сейчас находитесь, либо напишите вручную")
                .WithReplyKeyboard(new[]
                {
                    new [] { ShareCurrentLocation },
                    new [] { CancelButton }
                });

            public static readonly IMessage SpecifyAddress = MessageFactory.CreateCompositeMessage()
                .AddMessage(SpecifyAddressPrompt)
                .AddMessage(SpecifyAddressHint);

            public static readonly IMessage SpecifyParticipants = MessageFactory.CreateTextMessage()
                .WithPlainText("🛵 Участник ДТП")
                .WithReplyKeyboard(new[]
                {
                    new [] { new KeyboardButton("Мотоцикл"), new KeyboardButton("Мопед") },
                    new [] { new KeyboardButton("Велосипед") },
                    new [] { CancelButton }
                });

            public static readonly IMessage AreThereVictims = MessageFactory.CreateTextMessage()
                .WithPlainText("🤕 Есть пострадавшие?")
                .WithReplyKeyboard(new[]
                {
                    new [] { new KeyboardButton("Да"), new KeyboardButton("Нет") },
                    new [] { CancelButton }
                });

            private static readonly IMessage AskForContactsPrompt = MessageFactory.CreateTextMessage()
                .WithPlainText("📞 Номер для обратной связи");

            private static readonly IMessage AskForContactsHint = MessageFactory.CreateTextMessage()
                .WithMarkdownText($"Нажмите *{ShareNumber.Text}*, чтобы автоматически отправить свой номер телефона, либо напишите другой вручную")
                .WithReplyKeyboard(new[]
                {
                    new [] { ShareNumber, SkipButton },
                    new [] { CancelButton }
                });

            public static readonly IMessage AskForContacts = MessageFactory.CreateCompositeMessage()
                .AddMessage(AskForContactsPrompt)
                .AddMessage(AskForContactsHint);

            private static readonly IEnumerable<IEnumerable<KeyboardButton>> SummaryReplyKeyboard = new[]
            {
                new[] { SubmitButton },
                new[] { CancelButton }
            };

            public static IMessage ReportSummary(IAccidentReportDialogState state) => MessageFactory.CreateTextMessage()
                .WithInterpolatedMarkdownText(
@$"🚨 Сообщение о ДТП
    
 • *Адрес:* {state.Address ?? "Геопозиция"}
 • *Участник:* {state.Participant}
 • *Пострадавшие:* {state.Victims}
 • *Телефон:* {state.ReporterPhoneNumber}")
                .WithReplyKeyboard(SummaryReplyKeyboard);

            public static readonly IMessage ConfirmationExpected = MessageFactory.CreateTextMessage()
                .WithMarkdownText("🤔 Не совсем понял вас\n\n" +
                                  $"Нажмите *{SubmitButton.Text}*, чтобы сообщить о ДТП или *{CancelButton.Text}*, чтобы завершить без отправки")
                .WithReplyKeyboard(SummaryReplyKeyboard);

            public static readonly IMessage SuccessfullySent = MessageFactory.CreateTextMessage()
                .WithPlainText("✅ Успешно отправлено")
                .WithClearedReplyKeyboard();

            public static IMessage ReplyMaxLengthExceeded(int maxLength) => MessageFactory.CreateTextMessage()
                .WithMarkdownText($"😮 Максимальная длина ответа \\- *{maxLength}* символов, пожалуйста, сократите сообщение");
        }
    }
}