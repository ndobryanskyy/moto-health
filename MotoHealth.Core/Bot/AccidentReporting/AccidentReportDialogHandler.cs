using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
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

        private static readonly KeyboardButton CancelButton = new KeyboardButton("Отмена");

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
            var dialogTelemetry = _botTelemetry.GetTelemetryServiceForAccidentReporting(state);

            var cancelled = await TryHandleCancelButtonAsync();
            if (cancelled)
            {
                dialogTelemetry.OnCancelled();
                
                return true;
            }

            switch (state.CurrentStep)
            {
                case 0:
                {
                    await context.SendMessageAsync(Messages.SpecifyAddress, cancellationToken);
                    break;
                }

                case 1:
                {
                    var updateHandled = false;

                    if (context.Update is ITextMessageBotUpdate textMessage)
                    {
                        state.Address = textMessage.Text;
                        updateHandled = true;
                    } 
                    else if (context.Update is IMapLocation location)
                    {
                        dialogTelemetry.OnLocationSentAutomatically();

                        state.Location = location;
                        updateHandled = true;
                    }

                    if (updateHandled)
                    {
                        await context.SendMessageAsync(Messages.SpecifyParticipants, cancellationToken);
                        break;
                    }
                    else
                    {
                        // TODO handle wrong update type
                        dialogTelemetry.OnUnexpectedReply();

                        return false;
                    }
                }

                case 2:
                {
                    if (context.Update is ITextMessageBotUpdate textMessage)
                    {
                        state.Participant = textMessage.Text;

                        await context.SendMessageAsync(Messages.AreThereVictims, cancellationToken);
                        break;
                    }
                    else
                    {
                        // TODO handle wrong update type
                        dialogTelemetry.OnUnexpectedReply();

                        return false;
                    }
                }

                case 3:
                {
                    if (context.Update is ITextMessageBotUpdate textMessage)
                    {
                        state.Victims = textMessage.Text;

                        await context.SendMessageAsync(Messages.AskForContacts, cancellationToken);
                        break;
                    }
                    else
                    {
                        // TODO handle wrong update type
                        dialogTelemetry.OnUnexpectedReply();

                        return false;
                    }
                }

                case 4:
                {
                    var phoneNumber = context.Update switch
                    {
                        IContactMessageBotUpdate contactUpdate => contactUpdate.Contact.PhoneNumber,
                        ITextMessageBotUpdate textUpdate => textUpdate.Text,
                        _ => null
                    };

                    if (phoneNumber != null)
                    {
                        state.ReporterPhoneNumber = phoneNumber;

                        await context.SendMessageAsync(Messages.ReportSummaryWithPrompt(state), cancellationToken);

                        if (context.Update is IContactMessageBotUpdate)
                        {
                            dialogTelemetry.OnPhoneNumberSharedAutomatically();
                        }

                        break;
                    }
                    else
                    {
                        // TODO handle wrong update type
                        dialogTelemetry.OnUnexpectedReply();

                        return false;
                    }
                }

                case 5:
                {
                    if (context.Update is ITextMessageBotUpdate textMessage &&
                        textMessage.Text.Trim().Equals("да", StringComparison.InvariantCultureIgnoreCase))
                    {
                        await ReportAccidentAsync();

                        await context.SendMessageAsync(Messages.SuccessfullySent, cancellationToken);

                        dialogTelemetry.OnCompleted();

                        return true;
                    }
                    else
                    {
                        // TODO handle wrong update type and other negative answers
                        dialogTelemetry.OnUnexpectedReply();

                        return false;
                    }
                }


                default:
                    _logger.LogError("Got unexpected step number");
                    return true;
            }

            state.CurrentStep++;

            dialogTelemetry.OnNextStep();

            return false;

            async Task<bool> TryHandleCancelButtonAsync()
            {
                if (context.Update is ITextMessageBotUpdate textUpdate)
                {
                    if (textUpdate.Text == CancelButton.Text)
                    {
                        await context.SendMessageAsync(Messages.Canceled, cancellationToken);
                        return true;
                    }
                }

                return false;
            }

            async Task ReportAccidentAsync()
            {
                var report = AccidentReport.CreateFromDialogState(state);

                report.ReporterTelegramUserId = context.Update.Sender.Id;
                report.ReportedAtUtc = DateTime.UtcNow;

                await _accidentReportingService.ReportAccidentAsync(report, cancellationToken);
            }
        }

        private static class Messages
        {
            public static readonly IMessage Canceled = MessageFactory.CreateTextMessage()
                .WithPlainText("⛔ Отменено")
                .WithClearedReplyKeyboard();

            private static readonly IMessage SpecifyAddressPrompt = MessageFactory.CreateTextMessage()
                .WithPlainText("📍 Укажите адрес ДТП");

            private static readonly IMessage SpecifyAddressHint = MessageFactory.CreateTextMessage()
                .WithMarkdownText("💡 Нажмите *Мое местоположение*, чтобы автоматически отправить место на карте, гды Вы сейчас находитесь")
                .WithReplyKeyboard(new[]
                {
                    new [] { KeyboardButton.WithRequestLocation("Мое местоположение") },
                    new [] { CancelButton }
                });

            public static readonly IMessage SpecifyAddress = MessageFactory.CreateCompositeMessage()
                .AddMessage(SpecifyAddressPrompt)
                .AddMessage(SpecifyAddressHint);

            public static readonly IMessage SpecifyParticipants = MessageFactory.CreateTextMessage()
                .WithPlainText("🛵 Укажите участника ДТП")
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
                .WithPlainText("📞 Сообщить оператору Ваш номер телефона?");

            private static readonly IMessage AskForContactsHint = MessageFactory.CreateTextMessage()
                .WithMarkdownText("💡 Нажмите *Да* чтобы автоматически отправить номер")
                .WithReplyKeyboard(new[]
                {
                    new [] { KeyboardButton.WithRequestContact("Да"), new KeyboardButton("Нет") },
                    new [] { CancelButton }
                });

            public static readonly IMessage AskForContacts = MessageFactory.CreateCompositeMessage()
                .AddMessage(AskForContactsPrompt)
                .AddMessage(AskForContactsHint);

            public static IMessage ReportSummaryWithPrompt(IAccidentReportDialogState state) => MessageFactory.CreateTextMessage()
                .WithInterpolatedMarkdownText(
@$"🚨 Вы собираетесь сообщить о ДТП
    
 • *Адрес:* {state.Address ?? "Геопозиция"}
 • *Участник:* {state.Participant}
 • *Пострадавшие:* {state.Victims}
 • *Телефон:* {state.ReporterPhoneNumber}

_Отправить?_")
                .WithReplyKeyboard(new[]
                {
                    new [] { new KeyboardButton("Да") },
                    new [] { CancelButton }
                });

            public static readonly IMessage SuccessfullySent = MessageFactory.CreateTextMessage()
                .WithPlainText("✅ Успешно отправлено")
                .WithClearedReplyKeyboard();
        }
    }
}