using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Functions.AdminBot.ChatSubscriptions;
using Telegram.Bot.Types;

namespace MotoHealth.Functions.AdminBot
{
    public interface IAccidentAlertingService
    {
        /// <summary>
        /// Subscribes chat to the accidents topic
        /// </summary>
        /// <returns>True if new subscription was created</returns>
        Task<bool> SubscribeChatAsync(Chat chat);

        Task UnsubscribeChatAsync(Chat chat);
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
    }
}