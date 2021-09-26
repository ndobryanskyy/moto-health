using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.Commands.AppCommands
{
    internal sealed class SubscribeChatToAccidentAlertingBotCommand : ChatSubscriptionBotCommandBase
    {
        private static readonly IMessage ChatWasSubscribed = MessageFactory.CreateTextMessage()
            .WithPlainText("✅ Этот чат будет получать сообщения о ДТП");

        public SubscribeChatToAccidentAlertingBotCommand(
            IAuthorizationSecretsService secretsService,
            IChatSubscriptionsService subscriptionsService,
            IBotTelemetryService telemetryService)
            : base("/subscribe", secretsService, subscriptionsService, telemetryService)
        {
        }

        protected override async Task ExecuteAsync(IChatUpdateContext context, ICommandMessageBotUpdate command, CancellationToken cancellationToken)
        {
            await SubscriptionsService.SubscribeChatToTopicAsync(context.ChatId, AccidentAlertingTopicName, cancellationToken);

            await context.SendMessageAsync(ChatWasSubscribed, cancellationToken);

            TelemetryService.OnChatSubscribedToAccidentAlerting();
        }
    }
}