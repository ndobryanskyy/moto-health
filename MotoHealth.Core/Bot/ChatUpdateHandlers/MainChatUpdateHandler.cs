using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.ChatUpdateHandlers
{
    public sealed class MainChatUpdateHandler : ChatUpdateHandlerBase
    {
        private readonly IBotTelemetryService _telemetryService;
        private readonly IMainChatUpdateHandlerMessages _messages;
        private readonly IBotCommandsRegistry _commandsRegistry;
        private readonly IAccidentReportingDialogHandler _accidentReportingDialogHandler;

        public MainChatUpdateHandler(
            IBotTelemetryService telemetryService,
            IMainChatUpdateHandlerMessages messages,
            IBotCommandsRegistry commandsRegistry,
            IAccidentReportingDialogHandler accidentReportingDialogHandler)
        {
            _telemetryService = telemetryService;
            _messages = messages;
            _commandsRegistry = commandsRegistry;
            _accidentReportingDialogHandler = accidentReportingDialogHandler;
        }

        protected override bool SkipGroupUpdates => true;

        protected override bool SkipHandledUpdates => true;

        protected override async Task OnUpdateAsync(IChatUpdateContext context, ILogger logger, CancellationToken cancellationToken)
        {
            async Task SendMessageAsync(IMessage message)
            {
                await context.SendMessageAsync(message, cancellationToken);
            }

            var update = context.Update;

            switch (update)
            {
                case ICommandMessageBotUpdate commandMessage when _commandsRegistry.Start.Matches(commandMessage):
                {
                    await SendMessageAsync(_messages.Start);
                    context.IsUpdateHandled = true;

                    break;
                }
                case ICommandMessageBotUpdate commandMessage when _commandsRegistry.ReportAccident.Matches(commandMessage):
                {
                    await _accidentReportingDialogHandler.StartDialogAsync(context, cancellationToken);
                    context.IsUpdateHandled = true;

                    break;
                }
                case ICommandMessageBotUpdate commandMessage when _commandsRegistry.Info.Matches(commandMessage):
                {
                    await SendMessageAsync(_messages.MotoHealthInfo);

                    _telemetryService.OnMotoHealthInfoProvided();

                    context.IsUpdateHandled = true;

                    break;
                }
            }
        }
    }
}