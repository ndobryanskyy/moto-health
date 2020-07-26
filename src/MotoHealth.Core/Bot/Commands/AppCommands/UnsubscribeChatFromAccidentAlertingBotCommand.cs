using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.Commands.AppCommands
{
    internal sealed class UnsubscribeChatFromAccidentAlertingBotCommand : ChatSubscriptionBotCommandBase
    {
        private static readonly IMessage ChatWasUnsubscribed = MessageFactory.CreateTextMessage()
            .WithPlainText("⛔ Этот чат не будет получать сообщения о ДТП");

        public UnsubscribeChatFromAccidentAlertingBotCommand(
            IAuthorizationSecretsService secretsService,
            IChatSubscriptionsService subscriptionsService,
            IBotTelemetryService telemetryService) 
            : base("/unsubscribe", secretsService, subscriptionsService, telemetryService)
        {
        }

        protected override async Task ExecuteAsync(IChatUpdateContext context, CancellationToken cancellationToken)
        {
            await SubscriptionsService.UnsubscribeChatFromTopicAsync(context.ChatId, AccidentAlertingTopicName, cancellationToken);

            await context.SendMessageAsync(ChatWasUnsubscribed, cancellationToken);

            TelemetryService.OnChatUnsubscribedFromAccidentAlerting();
        }
    }
}