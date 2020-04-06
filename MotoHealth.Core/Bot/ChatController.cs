using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Messages;
using MotoHealth.Core.Bot.Updates;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatController : IChatController
    {
        public ChatController(IBotUpdateContext context, IChatState state, IMessageFactory messageFactory)
        {
            Context = context;
            State = state;
            MessageFactory = messageFactory;
        }

        private IChatState State { get; }

        private IBotUpdateContext Context { get; }

        private IMessageFactory MessageFactory { get; }

        public async Task HandleUpdateAsync(
            IAccidentReportDialogHandler accidentReportDialogHandler,
            CancellationToken cancellationToken)
        {
            if (State.AccidentReportDialog != null)
            {
                await HandleAccidentReportDialog(State.AccidentReportDialog);
            }
            else if (Context.Update is ICommandBotUpdate commandBotUpdate)
            {
                await HandleCommandAsync(commandBotUpdate);
            }
            else
            {
                await Context.SendMessageAsync(
                    MessageFactory.CreateTextMessage("..."),
                    cancellationToken
                );
            }

            async Task HandleCommandAsync(ICommandBotUpdate commandBotUpdate)
            {
                switch (commandBotUpdate.Command)
                {
                    case BotCommand.ReportAccident:
                        var dialogState = State.StartAccidentReportingDialog(1);

                        await HandleAccidentReportDialog(dialogState);
                        break;
                }
            }

            async Task HandleAccidentReportDialog(IAccidentReportDialogState dialogState)
            {
                if (await accidentReportDialogHandler.AdvanceDialogAsync(Context, dialogState, cancellationToken))
                {
                    State.CompleteAccidentReportingDialog();
                }
            }
        }
    }
}