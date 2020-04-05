using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates;
using MotoHealth.Core.Bot.Updates.Abstractions;
using Newtonsoft.Json;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatController : IChatController
    {
        public ChatController(IBotUpdateContext context, IChatState state)
        {
            Context = context;
            State = state;
        }

        private IChatState State { get; }

        private IBotUpdateContext Context { get; }

        public async Task HandleUpdateAsync(CancellationToken cancellationToken)
        {
            if (State.AccidentReportDialog != null)
            {
                await HandleAccidentReportDialog(
                    State.AccidentReportDialog,
                    cancellationToken
                );
            }
            else if (Context.Update is ICommandBotUpdate commandBotUpdate)
            {
                await HandleCommandAsync(commandBotUpdate, cancellationToken);
            }
            else
            {
                await Context.SendTextMessageAsync("...", cancellationToken);
            }
        }

        private async Task HandleCommandAsync(ICommandBotUpdate commandBotUpdate, CancellationToken cancellationToken)
        {
            switch (commandBotUpdate.Command)
            {
                case BotCommand.ReportAccident:
                    var dialog = State.StartAccidentReportingDialog(1);
                    
                    await HandleAccidentReportDialog(dialog, cancellationToken);
                    break;
            }
        }

        private async Task HandleAccidentReportDialog(
            IAccidentReportDialogState dialogState,
            CancellationToken cancellationToken)
        {
            switch (dialogState.CurrentStep)
            {
                case 1:
                    await Context.SendTextMessageAsync("📍 Укажите адрес ДТП", cancellationToken);
                    break;

                case 2:
                {
                    if (Context.Update is ITextMessageBotUpdate textMessage)
                    {
                        dialogState.Address = textMessage.Text;

                        await Context.SendTextMessageAsync("🛵 Укажите участников ДТП", cancellationToken);
                        break;
                    }
                    else
                    {
                        // TODO handle wrong update type
                        return;
                    }
                }

                case 3:
                {
                    if (Context.Update is ITextMessageBotUpdate textMessage)
                    {
                        dialogState.Participants = textMessage.Text;

                        await Context.SendTextMessageAsync("🤕 Есть пострадавшие?", cancellationToken);
                        break;
                    }
                    else
                    {
                        // TODO handle wrong update type
                        return;
                    }
                }

                case 4:
                {
                    if (Context.Update is ITextMessageBotUpdate textMessage)
                    {
                        dialogState.Victims = textMessage.Text;

                        await Context.SendTextMessageAsync("💬 Сообщить оператору Ваш номер телефона?", cancellationToken);
                        break;
                    }
                    else
                    {
                        // TODO handle wrong update type
                        return;
                    }
                }

                case 5:
                {
                    if (Context.Update is ITextMessageBotUpdate textMessage)
                    {
                        dialogState.ReporterPhoneNumber = textMessage.Text;

                        await Context.SendTextMessageAsync(JsonConvert.SerializeObject(dialogState), cancellationToken);

                        State.CompleteAccidentReportingDialog();

                        return;
                    }
                    else
                    {
                        // TODO handle wrong update type
                        return;
                    }
                }

                default:
                    // TODO Log and finish the dialog to prevent unrecoverable error
                    return;
            }

            dialogState.CurrentStep++;
        }
    }
}