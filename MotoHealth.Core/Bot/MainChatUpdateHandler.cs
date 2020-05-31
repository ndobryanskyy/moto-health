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
                    else if (update is ICommandMessageBotUpdate commandBotUpdate)
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
            ICommandMessageBotUpdate commandMessage, 
            CancellationToken cancellationToken)
        {
            if (_commands.Start.Matches(commandMessage))
            {
                await context.SendMessageAsync(Messages.Start, cancellationToken);
            }
            else if (_commands.ReportAccident.Matches(commandMessage))
            {
                var dialogState = state.StartAccidentReportingDialog(1);

                var dialogTelemetry = _botTelemetryService.GetTelemetryServiceForAccidentReporting(dialogState);

                dialogTelemetry.OnStarted();

                await HandleAccidentReportDialog(context, state, dialogState, cancellationToken);
            }
            else if (_commands.About.Matches(commandMessage))
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
            private static readonly IMessage StartCommandsHint = MessageFactory.CreateTextMessage()
                .WithMarkdownText("Нажмите /dtp чтобы получить помощь, если вы стали участником или свидетелем ДТП\n\nЭта и другие команды также доступны в меню *\\[ / \\]* внизу");

            private static readonly IMessage StartPinHint = MessageFactory.CreateTextMessage()
                .WithPlainText("📌 Чтобы не забыть про бота в экстренной ситуации, можете закрепите себе этот диалог");

            public static readonly IMessage Start = MessageFactory.CreateCompositeMessage()
                .AddMessage(StartCommandsHint)
                .AddMessage(StartPinHint);

            public static readonly IMessage MotoHealthInfo = MessageFactory.CreateTextMessage()
                .WithMarkdownText(
                    "Moto Health\n\n" +
                    "*Телефон:* \\+380960543434\n" +
                    "*Сайт:* [mh\\.od\\.ua](http://www.mh.od.ua)"
                );

            private static readonly IMessage NothingToSayHint = MessageFactory.CreateTextMessage()
                .WithMarkdownText("Попробуйте выбрать команду в меню *\\[ / \\]* внизу");

            public static readonly IMessage NothingToSay = MessageFactory.CreateCompositeMessage()
                .AddMessage(CommonMessages.NotQuiteGetIt)
                .AddMessage(NothingToSayHint);

            public static readonly IMessage PleaseTryLater = MessageFactory.CreateTextMessage()
                .WithPlainText("😥 Ой, что-то пошло не так\n\nПопробуйте ещё раз через пару секунд");
        }
    }
}