using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram.Messages;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Core.Bot
{
    internal sealed class MainChatUpdateHandler : ChatUpdateHandlerBase
    {
        private readonly ILogger<MainChatUpdateHandler> _logger;
        private readonly IBotCommandsRegistry _commands;
        private readonly IAccidentReportDialogHandler _accidentReportDialogHandler;
        private readonly IBotTelemetryService _botTelemetryService;
        private readonly IAdminCommandsHandler _adminCommandsHandler;

        public MainChatUpdateHandler(
            ILogger<MainChatUpdateHandler> logger,
            IBotCommandsRegistry commands, 
            IAccidentReportDialogHandler accidentReportDialogHandler,
            IBotTelemetryService botTelemetryService,
            IAdminCommandsHandler adminCommandsHandler,
            IMainChatMessages messages)
        {
            _logger = logger;
            _commands = commands;
            _accidentReportDialogHandler = accidentReportDialogHandler;
            _botTelemetryService = botTelemetryService;
            _adminCommandsHandler = adminCommandsHandler;

            Messages = messages;
        }

        private IMainChatMessages Messages { get; }

        protected override async Task OnUpdateAsync(IChatUpdateContext context, CancellationToken cancellationToken)
        {
            async Task SendMessageAsync(IMessage message)
            {
                await context.SendMessageAsync(message, cancellationToken);
            }

            var update = context.Update;

            if (TryLockChat(out var chatLock))
            {
                using (chatLock)
                {
                    var state = await GetStateAsync(cancellationToken);

                    await OnSynchronizedUpdateAsync(state);
                }
            }
            else
            {
                _botTelemetryService.OnChatIsStillLocked();

                await SendMessageAsync(Messages.PleaseTryLater);
            }

            async Task OnSynchronizedUpdateAsync(IChatState state)
            {
                if (await _adminCommandsHandler.TryHandleUpdateAsync(context, cancellationToken))
                {
                    return;
                }

                if (update.Chat.Type != ChatType.Private)
                {
                    _logger.LogWarning("Skipping group chat update");
                    return;
                }

                var accidentReportDialog = state.AccidentReportDialog;
                if (accidentReportDialog != null)
                {
                    await HandleAccidentReportDialog(accidentReportDialog);
                }
                else if (update is ICommandMessageBotUpdate commandBotUpdate)
                {
                    await HandleCommandAsync(commandBotUpdate);
                }
                else
                {
                    await OnNothingToSayAsync();
                }

                await UpdateStateAsync(state, cancellationToken);

                async Task HandleCommandAsync(ICommandMessageBotUpdate commandMessage)
                {
                    if (_commands.Start.Matches(commandMessage))
                    {
                        await SendMessageAsync(Messages.Start);
                    }
                    else if (_commands.ReportAccident.Matches(commandMessage))
                    {
                        var dialogState = state.StartAccidentReportingDialog(1);

                        var dialogTelemetry = _botTelemetryService.GetTelemetryServiceForAccidentReporting(dialogState);

                        dialogTelemetry.OnStarted();

                        await HandleAccidentReportDialog(dialogState);
                    }
                    else if (_commands.About.Matches(commandMessage))
                    {
                        await SendMessageAsync(Messages.MotoHealthInfo);

                        _botTelemetryService.OnMotoHealthInfoProvided();
                    }
                    else
                    {
                        await OnNothingToSayAsync();
                    }
                }

                async Task HandleAccidentReportDialog(IAccidentReportDialogState dialogState)
                {
                    if (await _accidentReportDialogHandler.AdvanceDialogAsync(context, dialogState, cancellationToken))
                    {
                        state.CompleteAccidentReportingDialog();
                    }
                }

                async Task OnNothingToSayAsync()
                {
                    _botTelemetryService.OnNothingToSay();

                    await SendMessageAsync(Messages.NothingToSay);
                }
            }
        }
    }
}