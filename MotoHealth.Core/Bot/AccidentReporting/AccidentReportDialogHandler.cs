using System;
using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Messages;
using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public interface IAccidentReportDialogHandler
    {
        Task<bool> AdvanceDialogAsync(IChatUpdateContext context, IAccidentReportDialogState state, CancellationToken cancellationToken);
    }

    internal sealed class AccidentReportDialogHandler : IAccidentReportDialogHandler
    {
        private readonly IAccidentsQueue _accidentsQueue;
        private static readonly KeyboardButton CancelButton = new KeyboardButton("Отмена");

        public AccidentReportDialogHandler(IAccidentsQueue accidentsQueue)
        {
            _accidentsQueue = accidentsQueue;
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
            var cancelled = await TryHandleCancelButtonAsync();
            if (cancelled) return true;

            switch (state.CurrentStep)
            {
                case 1:
                {
                    await context.SendMessageAsync(Messages.SpecifyAddress, cancellationToken);
                    break;
                }

                case 2:
                {
                    if (context.Update is ITextMessageBotUpdate textMessage)
                    {
                        state.Address = textMessage.Text;

                        await context.SendMessageAsync(Messages.SpecifyParticipants, cancellationToken);
                        break;
                    }
                    else
                    {
                        // TODO handle wrong update type
                        return false;
                    }
                }

                case 3:
                {
                    if (context.Update is ITextMessageBotUpdate textMessage)
                    {
                        state.Participants = textMessage.Text;

                        await context.SendMessageAsync(Messages.AreThereVictims, cancellationToken);
                        break;
                    }
                    else
                    {
                        // TODO handle wrong update type
                        return false;
                    }
                }

                case 4:
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
                        return false;
                    }
                }

                case 5:
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
                        break;
                    }
                    else
                    {
                        // TODO handle wrong update type
                        return false;
                    }
                }

                case 6:
                    {
                        if (context.Update is ITextMessageBotUpdate textMessage &&
                            textMessage.Text.Trim().Equals("да", StringComparison.InvariantCultureIgnoreCase))
                        {
                            await AddReportToQueueAsync();

                            await context.SendMessageAsync(Messages.SuccessfullySent, cancellationToken);
                            return true;
                        }
                        else
                        {
                            // TODO handle wrong update type and other negative answers
                            return false;
                        }
                    }


                default:
                    // TODO Log alert and finish the dialog
                    return true;
            }

            state.CurrentStep++;

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

            async Task AddReportToQueueAsync()
            {
                var report = new AccidentReport(
                    state.InstanceId,
                    context.Update.Sender.Id,
                    DateTime.UtcNow, 
                    state.Address,
                    state.Participants,
                    state.Victims,
                    state.ReporterPhoneNumber ?? "Нет"
                );

                await _accidentsQueue.EnqueueReportAsync(report, cancellationToken);
            }
        }

        private static class Messages
        {
            public static readonly IMessage Canceled = MessageFactory.CreateTextMessage()
                .WithPlainText("⛔ Отменено")
                .WithClearedReplyKeyboard();

            public static readonly IMessage SpecifyAddress = MessageFactory.CreateTextMessage()
                .WithPlainText("📍 Укажите адрес ДТП")
                .WithReplyKeyboard(new[]
                {
                    new [] { CancelButton }
                });

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

            public static readonly IMessage AskForContacts = MessageFactory.CreateTextMessage()
                .WithMarkdownText("📞 Сообщить оператору Ваш номер телефона?\n\n\n" +
                                   "💡 _Нажмите *да* чтобы автоматически отправить номер_")
                .WithReplyKeyboard(new[]
                {
                    new [] { KeyboardButton.WithRequestContact("Да"), new KeyboardButton("Нет") },
                    new [] { CancelButton }
                });

            public static IMessage ReportSummaryWithPrompt(IAccidentReportDialogState state) => MessageFactory.CreateTextMessage()
                .WithInterpolatedMarkdownText(
@$"🚨 Вы собираетесь сообщить о ДТП
    
 • *Адрес:* {state.Address}
 • *Участники:* {state.Participants}
 • *Есть жертвы:* {state.Victims}
 • *Телефон:* {state.ReporterPhoneNumber}

_Отправить?_", true)
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