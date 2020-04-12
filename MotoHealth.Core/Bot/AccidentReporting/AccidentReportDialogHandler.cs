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
        Task<bool> AdvanceDialogAsync(IBotUpdateContext context, IAccidentReportDialogState state, CancellationToken cancellationToken);
    }

    internal sealed class AccidentReportDialogHandler : IAccidentReportDialogHandler
    {
        private readonly IAccidentsQueue _accidentsQueue;
        private static readonly KeyboardButton CancelButton = new KeyboardButton("Отмена");
        
        private readonly Messages _messages;

        public AccidentReportDialogHandler(IMessageFactory messageFactory, IAccidentsQueue accidentsQueue)
        {
            _accidentsQueue = accidentsQueue;
            _messages = new Messages(messageFactory);
        }

        /// <summary>
        /// Advances dialog with specified context and state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="state"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if dialog ended.</returns>
        public async Task<bool> AdvanceDialogAsync(
            IBotUpdateContext context,
            IAccidentReportDialogState state,
            CancellationToken cancellationToken)
        {
            var cancelled = await TryHandleCancelButtonAsync();
            if (cancelled) return true;

            switch (state.CurrentStep)
            {
                case 1:
                {
                    await context.SendMessageAsync(_messages.SpecifyAddress, cancellationToken);
                    break;
                }

                case 2:
                {
                    if (context.Update is ITextMessageBotUpdate textMessage)
                    {
                        state.Address = textMessage.Text;

                        await context.SendMessageAsync(_messages.SpecifyParticipants, cancellationToken);
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

                        await context.SendMessageAsync(_messages.AreThereVictims, cancellationToken);
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

                        await context.SendMessageAsync(_messages.AskForContacts, cancellationToken);
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

                        await context.SendMessageAsync(_messages.ReportSummaryWithPrompt(state), cancellationToken);
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

                            await context.SendMessageAsync(_messages.SuccessfullySent, cancellationToken);
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
                        await context.SendMessageAsync(_messages.Canceled, cancellationToken);
                        return true;
                    }
                }

                return false;
            }

            async Task AddReportToQueueAsync()
            {
                var report = new AccidentReport(
                    state.InstanceId,
                    context.Update.Chat.From.Id,
                    DateTime.UtcNow, 
                    state.Address,
                    state.Participants,
                    state.Victims,
                    state.ReporterPhoneNumber ?? "Нет"
                );

                await _accidentsQueue.EnqueueReportAsync(report, cancellationToken);
            }
        }

        private sealed class Messages
        {
            private readonly IMessageFactory _messageFactory;

            public Messages(IMessageFactory messageFactory)
            {
                _messageFactory = messageFactory;
            }

            public IMessage Canceled => _messageFactory.CreateTextMessage()
                .WithPlainText("⛔ Отменено")
                .WithClearedReplyKeyboard();

            public IMessage SpecifyAddress => _messageFactory.CreateTextMessage()
                .WithPlainText("📍 Укажите адрес ДТП")
                .WithReplyKeyboard(new[]
                {
                    new [] { CancelButton }
                });

            public IMessage SpecifyParticipants => _messageFactory.CreateTextMessage()
                .WithPlainText("🛵 Укажите участника ДТП")
                .WithReplyKeyboard(new[]
                {
                    new [] { new KeyboardButton("Мотоцикл"), new KeyboardButton("Мопед") },
                    new [] { new KeyboardButton("Велосипед") },
                    new [] { CancelButton }
                });

            public IMessage AreThereVictims => _messageFactory.CreateTextMessage()
                .WithPlainText("🤕 Есть пострадавшие?")
                .WithReplyKeyboard(new[]
                {
                    new [] { new KeyboardButton("Да"), new KeyboardButton("Нет") },
                    new [] { CancelButton }
                });

            public IMessage AskForContacts => _messageFactory.CreateTextMessage()
                .WithMarkdownText("📞 Сообщить оператору Ваш номер телефона?\n\n\n" +
                                   "💡 _Нажмите *да* чтобы автоматически отправить номер_")
                .WithReplyKeyboard(new[]
                {
                    new [] { KeyboardButton.WithRequestContact("Да"), new KeyboardButton("Нет") },
                    new [] { CancelButton }
                });

            public IMessage ReportSummaryWithPrompt(IAccidentReportDialogState state) => _messageFactory.CreateTextMessage()
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

            public IMessage SuccessfullySent => _messageFactory.CreateTextMessage()
                .WithPlainText("✅ Успешно отправлено")
                .WithClearedReplyKeyboard();
        }
    }
}