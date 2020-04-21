using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Core.Bot.Messages;
using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Core.Bot
{
    internal sealed class MainChatUpdateHandler : ChatUpdateHandlerBase
    {
        private readonly ILogger<MainChatUpdateHandler> _logger;
        private readonly IBotCommandsRegistry _commands;
        private readonly IAccidentReportDialogHandler _accidentReportDialogHandler;

        public MainChatUpdateHandler(
            ILogger<MainChatUpdateHandler> logger,
            IBotCommandsRegistry commands, 
            IAccidentReportDialogHandler accidentReportDialogHandler)
        {
            _logger = logger;
            _commands = commands;
            _accidentReportDialogHandler = accidentReportDialogHandler;
        }

        protected override async Task OnUpdateAsync(IChatUpdateContext context, CancellationToken cancellationToken)
        {
            var update = context.Update;

            if (update.Chat.Type != ChatType.Private)
            {
                _logger.LogInformation($"Skipping group chat update {update.UpdateId}");
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
                        await context.SendMessageAsync(Messages.NothingToRespondMessage, cancellationToken);
                    }

                    await UpdateStateAsync(state, cancellationToken);
                }
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

                await HandleAccidentReportDialog(context, state, dialogState, cancellationToken);
            }
            else if (_commands.About.Matches(commandBotUpdate))
            {
                await context.SendMessageAsync(Messages.MotoHealthInfoMessage, cancellationToken);
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

        private static class Messages
        {
            public static readonly IMessage MotoHealthInfoMessage = MessageFactory.CreateTextMessage()
                .WithMarkdownText(
                    "Moto Health Odessa\n\n" +
                    "*Телефон:* \\+380960543434\n" +
                    "*Сайт:* [mh\\.od\\.ua](http://www.mh.od.ua)"
                );

            public static readonly IMessage NothingToRespondMessage = MessageFactory.CreateTextMessage()
                .WithPlainText("...");
        }
    }
}