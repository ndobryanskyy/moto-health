using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Core.Bot.Commands.AppCommands
{
    [PublicBotCommand(Name, "Сообщить о ДТП")]
    internal sealed class ReportAccidentBotCommand : PrivateChatBotCommandBase
    {
        private const string Name = "/dtp";

        private readonly IAccidentReportingDialogHandler _dialogHandler;

        public ReportAccidentBotCommand(IAccidentReportingDialogHandler dialogHandler)
            : base(Name)
        {
            _dialogHandler = dialogHandler;
        }

        protected override async Task ExecuteAsync(IChatUpdateContext context, CancellationToken cancellationToken)
        {
            var state = await context.GetStagingStateAsync(cancellationToken);

            if (state.AccidentReportDialog == null)
            {
                await _dialogHandler.StartDialogAsync(context, cancellationToken);
            }
        }
    }
}