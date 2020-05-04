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

        public MainChatUpdateHandler(
            ILogger<MainChatUpdateHandler> logger,
            IBotCommandsRegistry commands, 
            IAccidentReportDialogHandler accidentReportDialogHandler,
            IBotTelemetryService botTelemetryService)
        {
            _logger = logger;
            _commands = commands;
            _accidentReportDialogHandler = accidentReportDialogHandler;
            _botTelemetryService = botTelemetryService;
        }

        protected override async Task OnUpdateAsync(IChatUpdateContext context, CancellationToken cancellationToken)
        {
            var update = context.Update;

            if (update.Chat.Type != ChatType.Private)
            {
                _logger.LogWarning("Skipping group chat update");
                return;
            }

            if (TryLockChat(out var chatLock))
            {
                using (chatLock)
                {
                    var state = await GetStateAsync(cancellationToken);

                    var accidentReportDialog = state.AccidentReportDialog;
                    if (accidentReportDialog != null)
                    {
                        await HandleAccidentReportDialog(context, state, accidentReportDialog, cancellationToken);
                    }
                    else if (update is ICommandBotUpdate commandBotUpdate)
                    {
                        await HandleCommandAsync(context, state, commandBotUpdate, cancellationToken);
                    }
                    else
                    {
                        await OnNothingToSayAsync(context, cancellationToken);
                    }

                    await UpdateStateAsync(state, cancellationToken);
                }
            }
            else
            {
                _botTelemetryService.OnChatIsStillLocked();

                await context.SendMessageAsync(Messages.PleaseTryLater, cancellationToken);
            }
        }

        private async Task HandleCommandAsync(
            IChatUpdateContext context, 
            IChatState state, 
            ICommandBotUpdate commandBotUpdate, 
            CancellationToken cancellationToken)
        {
            if (_commands.ReportAccident.Matches(commandBotUpdate))
            {
                var dialogState = state.StartAccidentReportingDialog(1);

                var dialogTelemetry = _botTelemetryService.GetTelemetryServiceForAccidentReporting(dialogState);

                dialogTelemetry.OnStarted();

                await HandleAccidentReportDialog(context, state, dialogState, cancellationToken);
            }
            else if (_commands.About.Matches(commandBotUpdate))
            {
                await context.SendMessageAsync(Messages.MotoHealthInfo, cancellationToken);

                _botTelemetryService.OnMotoHealthInfoProvided();
            }
            else
            {
                await OnNothingToSayAsync(context, cancellationToken);
            }
        }

        private async Task HandleAccidentReportDialog(
            IChatUpdateContext context,
            IChatState state, 
            IAccidentReportDialogState dialogState,
            CancellationToken cancellationToken)
        {
            if (await _accidentReportDialogHandler.AdvanceDialogAsync(context, dialogState, cancellationToken))
            {
                state.CompleteAccidentReportingDialog();
            }
        }

        private async Task OnNothingToSayAsync(IChatUpdateContext context, CancellationToken cancellationToken)
        {
            _botTelemetryService.OnNothingToSay();

            await context.SendMessageAsync(Messages.NothingToSay, cancellationToken);
        }

        private static class Messages
        {
            public static readonly IMessage MotoHealthInfo = MessageFactory.CreateTextMessage()
                .WithMarkdownText(
                    "Moto Health Odessa\n\n" +
                    "*Телефон:* \\+380960543434\n" +
                    "*Сайт:* [mh\\.od\\.ua](http://www.mh.od.ua)"
                );

            public static readonly IMessage NothingToSay = MessageFactory.CreateTextMessage()
                .WithPlainText("...");

            public static readonly IMessage PleaseTryLater = MessageFactory.CreateTextMessage()
                .WithPlainText("😥 Извините, но что-то пошло не так\n\nПопробуйте ещё раз через пару секунд");
        }
    }
}