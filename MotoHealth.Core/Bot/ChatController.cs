using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Core.Bot.Messages;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatController : IChatController
    {
        private readonly IBotCommandsRegistry _commandsRegistry;

        public ChatController(
            IBotUpdateContext context, 
            IChatState state, 
            IMessageFactory messageFactory,
            IBotCommandsRegistry commandsRegistry)
        {
            _commandsRegistry = commandsRegistry;

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
                if (_commandsRegistry.ReportAccident.Matches(commandBotUpdate))
                {
                    var dialogState = State.StartAccidentReportingDialog(1);

                    await HandleAccidentReportDialog(dialogState);
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