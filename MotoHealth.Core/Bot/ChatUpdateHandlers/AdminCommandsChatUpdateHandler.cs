using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.ChatUpdateHandlers
{
    public sealed class AdminCommandsChatUpdateHandler : ChatUpdateHandlerBase
    {
        private const string AccidentAlertingTopic = "Accidents";

        private readonly IBotTelemetryService _telemetryService;
        private readonly IAdminCommandsChatUpdateHandlerMessages _messages;
        private readonly IBotCommandsRegistry _commandsRegistry;
        private readonly IAuthorizationSecretsService _secretsService;
        private readonly IChatSubscriptionsService _chatSubscriptionsService;

        public AdminCommandsChatUpdateHandler(
            IBotTelemetryService telemetryService,
            IAdminCommandsChatUpdateHandlerMessages messages,
            IBotCommandsRegistry commandsRegistry,
            IAuthorizationSecretsService secretsService,
            IChatSubscriptionsService chatSubscriptionsService)
        {
            _telemetryService = telemetryService;
            _messages = messages;
            _commandsRegistry = commandsRegistry;
            _secretsService = secretsService;
            _chatSubscriptionsService = chatSubscriptionsService;
        }

        protected override bool SkipGroupUpdates => false;

        protected override bool SkipHandledUpdates => true;

        protected override async Task OnUpdateAsync(IChatUpdateContext context, ILogger logger, CancellationToken cancellationToken)
        {
            async Task SendMessageAsync(IMessage message)
            {
                await context.SendMessageAsync(message, cancellationToken);
            }

            switch (context.Update)
            {
                case ICommandMessageBotUpdate commandMessage
                     when _commandsRegistry.SubscribeChat.Matches(commandMessage, out var secret) &&
                     _secretsService.VerifySubscriptionSecret(secret):
                    {
                        await _chatSubscriptionsService.SubscribeChatToTopicAsync(context.ChatId, AccidentAlertingTopic, cancellationToken);

                        await SendMessageAsync(_messages.ChatSubscribed);

                        logger.LogDebug("Chat subscribed to accident alerts");
                        _telemetryService.OnChatSubscribedToAccidentAlerting();

                        context.IsUpdateHandled = true;

                        break;
                    }

                case ICommandMessageBotUpdate commandMessage
                    when _commandsRegistry.UnsubscribeChat.Matches(commandMessage, out var secret) &&
                         _secretsService.VerifySubscriptionSecret(secret):
                    {
                        await _chatSubscriptionsService.UnsubscribeChatFromTopicAsync(context.ChatId, AccidentAlertingTopic, cancellationToken);

                        await SendMessageAsync(_messages.ChatUnsubscribed);

                        logger.LogDebug("Chat unsubscribed from accident alerts");
                        _telemetryService.OnChatUnsubscribedFromAccidentAlerting();

                        context.IsUpdateHandled = true;

                        break;
                    }
            }
        }
    }
}