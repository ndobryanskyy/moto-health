using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Functions.AdminBot.ChatSubscriptions;

namespace MotoHealth.Functions.AdminBot
{
    public interface IAccidentAlertingService
    {
        /// <summary>
        /// Subscribes chat to the accidents topic
        /// </summary>
        /// <returns>True if new subscription was created</returns>
        Task<bool> SubscribeChatAsync(long chatId);

        Task UnsubscribeChatAsync(long chatId);
    }

    internal sealed class AccidentAlertingService : IAccidentAlertingService
    {
        private const string SubscriptionTopic = "Accidents";

        private readonly ILogger<AccidentAlertingService> _logger;
        private readonly IChatSubscriptionsManager _chatSubscriptionsManager;

        public AccidentAlertingService(
            ILogger<AccidentAlertingService> logger,
            IChatSubscriptionsManager chatSubscriptionsManager)
        {
            _logger = logger;
            _chatSubscriptionsManager = chatSubscriptionsManager;
        }

        public async Task<bool> SubscribeChatAsync(long chatId)
        {
            var alreadySubscribed = await _chatSubscriptionsManager.CheckIfChatIsSubscribedToTopicAsync(chatId, SubscriptionTopic);

            if (alreadySubscribed)
            {
                _logger.LogInformation($"Tried to subscribe already subscribed chat {chatId}");

                return false;
            }

            await _chatSubscriptionsManager.SubscribeChatToTopicAsync(chatId, SubscriptionTopic);

            _logger.LogInformation($"Chat {chatId} now subscribed");

            return true;
        }

        public async Task UnsubscribeChatAsync(long chatId)
        {
            var isCurrentlySubscribed = await _chatSubscriptionsManager.CheckIfChatIsSubscribedToTopicAsync(chatId, SubscriptionTopic);

            if (!isCurrentlySubscribed)
            {
                _logger.LogWarning($"Tried to unsubscribe chat {chatId}, that is not yet subscribed");
                
                return;
            }

            await _chatSubscriptionsManager.UnsubscribeChatFromTopicAsync(chatId, SubscriptionTopic);

            _logger.LogInformation($"Chat {chatId} now unsubscribed");
        }
    }
}