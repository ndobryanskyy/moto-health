using System;
using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Messages;
using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;

namespace MotoHealth.Core.Bot
{
    public interface IAccidentReportDialogHandler
    {
        Task<bool> AdvanceDialogAsync(IBotUpdateContext context, IAccidentReportDialogState state, CancellationToken cancellationToken);
    }

    internal sealed class AccidentReportDialogHandler : IAccidentReportDialogHandler
    {
        private static readonly KeyboardButton CancelButton = new KeyboardButton("ОТМЕНА");
        
        private readonly Messages _messages;

        public AccidentReportDialogHandler(IMessageFactory messageFactory)
        {
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
            var cancelled = await TryHandleCancelButtonAsync(context, cancellationToken);
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
        }

        private async Task<bool> TryHandleCancelButtonAsync(IBotUpdateContext context, CancellationToken cancellationToken)
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

        private sealed class Messages
        {
            private readonly IMessageFactory _messageFactory;

            public Messages(IMessageFactory messageFactory)
            {
                _messageFactory = messageFactory;
            }

            public IMessage Canceled => _messageFactory
                .CreateTextMessage("⛔ Отменено")
                .WithClearedReplyKeyboard();

            public IMessage SpecifyAddress => _messageFactory
                .CreateTextMessage("📍 Укажите адрес ДТП")
                .WithReplyKeyboard(new[]
                {
                    new [] { CancelButton }
                });

            public IMessage SpecifyParticipants => _messageFactory
                .CreateTextMessage("🛵 Укажите участников ДТП")
                .WithReplyKeyboard(new[]
                {
                    new [] { new KeyboardButton("Мотоцикл"), new KeyboardButton("Мопед") },
                    new [] { new KeyboardButton("Велосипед") },
                    new [] { CancelButton }
                });

            public IMessage AreThereVictims => _messageFactory
                .CreateTextMessage("🤕 Есть пострадавшие?")
                .WithReplyKeyboard(new[]
                {
                    new [] { new KeyboardButton("Да"), new KeyboardButton("Нет") },
                    new [] { CancelButton }
                });

            public IMessage AskForContacts => _messageFactory
                .CreateTextMessage("💬 Сообщить оператору Ваш номер телефона?")
                .WithReplyKeyboard(new[]
                {
                    new [] { KeyboardButton.WithRequestContact("Да"), new KeyboardButton("Нет") },
                    new [] { CancelButton }
                });

            public IMessage ReportSummaryWithPrompt(IAccidentReportDialogState state) => _messageFactory
                .CreateTextMessage("Вы собираетесь сообщить о ДТП\n\n" +
                                   $" • *Адрес:* {state.Address}\n" +
                                   $" • *Участники:* {state.Participants}\n" +
                                   $" • *Есть жертвы:* {state.Victims}\n" +
                                   $" • *Телефон:* {state.ReporterPhoneNumber}\n\n" +
                                   $"_Отправить?_")
                .ParseAsMarkdown()
                .WithReplyKeyboard(new[]
                {
                    new [] { new KeyboardButton("Да") },
                    new [] { CancelButton }
                });

            public IMessage SuccessfullySent => _messageFactory
                .CreateTextMessage("✅ Успешно отправлено")
                .WithClearedReplyKeyboard();
        }
    }
}