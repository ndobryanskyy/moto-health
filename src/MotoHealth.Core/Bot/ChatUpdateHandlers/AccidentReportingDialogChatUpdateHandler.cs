using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Core.Bot.ChatUpdateHandlers
{
    public sealed class AccidentReportingDialogChatUpdateHandler : ChatUpdateHandlerBase
    {
        private readonly IAccidentReportingDialogHandler _dialogHandler;

        public AccidentReportingDialogChatUpdateHandler(IAccidentReportingDialogHandler dialogHandler)
        {
            _dialogHandler = dialogHandler;
        }

        protected override bool SkipGroupUpdates => true;
        
        protected override bool SkipHandledUpdates => true;

        protected override async ValueTask OnUpdateAsync(IChatUpdateContext context, ILogger logger, CancellationToken cancellationToken)
        {
            var state = await context.GetStagingStateAsync(cancellationToken);

            if (state.AccidentReportDialog != null)
            {
                await _dialogHandler.AdvanceDialogAsync(context, cancellationToken);
                context.IsUpdateHandled = true;
            }
        }
    }
}