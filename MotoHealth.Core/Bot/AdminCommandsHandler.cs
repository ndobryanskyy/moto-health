using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot
{
    public interface IAdminCommandsHandler
    {
        Task<bool> TryHandleUpdateAsync(IChatUpdateContext context, CancellationToken cancellationToken);
    }

    internal sealed class AdminCommandsHandler : IAdminCommandsHandler
    {
        private const string AccidentAlertingTopic = "Accidents";

        private readonly ILogger<AdminCommandsHandler> _logger;
        private readonly IBotTelemetryService _botTelemetryService;
        private readonly IChatSubscriptionsService _chatSubscriptionsService;
        private readonly IAuthorizationSecretsService _authorizationSecretsService;
        private readonly IBotCommandsRegistry _commandsRegistry;

        public AdminCommandsHandler(
            ILogger<AdminCommandsHandler> logger,
            IBotTelemetryService botTelemetryService,
            IAdminHandlerMessages messages,
            IChatSubscriptionsService chatSubscriptionsService,
            IAuthorizationSecretsService authorizationSecretsService,
            IBotCommandsRegistry commandsRegistry)
        {
            _logger = logger;
            _botTelemetryService = botTelemetryService;
            _chatSubscriptionsService = chatSubscriptionsService;
            _authorizationSecretsService = authorizationSecretsService;
            _commandsRegistry = commandsRegistry;

            Messages = messages;
        }

        private IAdminHandlerMessages Messages { get; }

        public async Task<bool> TryHandleUpdateAsync(IChatUpdateContext context, CancellationToken cancellationToken)
        {
            var update = context.Update;
            var chatId = update.Chat.Id;

            if (update is ICommandMessageBotUpdate commandMessage)
            {
                if (_commandsRegistry.SubscribeChat.Matches(commandMessage, out var secret)
                    && _authorizationSecretsService.VerifySubscriptionSecret(secret))
                {
                    await _chatSubscriptionsService.SubscribeChatToTopicAsync(chatId, AccidentAlertingTopic, cancellationToken);

                    await context.SendMessageAsync(Messages.ChatSubscribed, cancellationToken);

                    _logger.LogInformation($"Handled update {update.UpdateId}");
                    _botTelemetryService.OnChatSubscribedToAccidentAlerting();

                    return true;
                }
                else if (_commandsRegistry.UnsubscribeChat.Matches(commandMessage, out secret)
                         && _authorizationSecretsService.VerifySubscriptionSecret(secret))
                {
                    await _chatSubscriptionsService.UnsubscribeChatFromTopicAsync(chatId, AccidentAlertingTopic, cancellationToken);

                    await context.SendMessageAsync(Messages.ChatUnsubscribed, cancellationToken);

                    _logger.LogInformation($"Handled update {update.UpdateId}");
                    _botTelemetryService.OnChatUnsubscribedFromAccidentAlerting();

                    return true;
                }
            }
            
            _logger.LogInformation($"Skipping update {update.UpdateId}");
            
            return false;
        }
    }
}