using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Functions.ChatSubscriptions;
using Telegram.Bot.Types;

namespace MotoHealth.Functions.AccidentAlerting
{
    public interface IAccidentAlertingSubscriptionsManager
    {
        /// <summary>
        /// Subscribes chat to the accidents topic
        /// </summary>
        /// <returns>True if new subscription was created</returns>
        Task<bool> SubscribeChatAsync(Chat chat);

        Task UnsubscribeChatAsync(Chat chat);

        Task<ChatSubscription[]> GetSubscribedChatsAsync();
    }

    internal sealed class AccidentAlertingSubscriptionsManager : IAccidentAlertingSubscriptionsManager
    {
        private const string SubscriptionTopic = "Accidents";

        private readonly ILogger<AccidentAlertingSubscriptionsManager> _logger;
        private readonly IChatSubscriptionsManager _chatSubscriptionsManager;

        public AccidentAlertingSubscriptionsManager(
            ILogger<AccidentAlertingSubscriptionsManager> logger,
            IChatSubscriptionsManager chatSubscriptionsManager)
        {
            _logger = logger;
            _chatSubscriptionsManager = chatSubscriptionsManager;
        }

        public async Task<bool> SubscribeChatAsync(Chat chat)
        {
            var alreadySubscribed = await _chatSubscriptionsManager.CheckIfChatIsSubscribedToTopicAsync(chat, SubscriptionTopic);

            if (alreadySubscribed)
            {
                _logger.LogInformation($"Tried to subscribe already subscribed chat {chat.Id}");

                return false;
            }

            await _chatSubscriptionsManager.SubscribeChatToTopicAsync(chat, SubscriptionTopic);

            _logger.LogInformation($"Chat {chat.Id} now subscribed");

            return true;
        }

        public async Task UnsubscribeChatAsync(Chat chat)
        {
            var isCurrentlySubscribed = await _chatSubscriptionsManager.CheckIfChatIsSubscribedToTopicAsync(chat, SubscriptionTopic);

            if (!isCurrentlySubscribed)
            {
                _logger.LogWarning($"Tried to unsubscribe chat {chat.Id}, that is not yet subscribed");
                
                return;
            }

            await _chatSubscriptionsManager.UnsubscribeChatFromTopicAsync(chat, SubscriptionTopic);

            _logger.LogInformation($"Chat {chat.Id} now unsubscribed");
        }

        public async Task<ChatSubscription[]> GetSubscribedChatsAsync()
        {
            return await _chatSubscriptionsManager.GetTopicSubscriptions(SubscriptionTopic);
        }
    }
}