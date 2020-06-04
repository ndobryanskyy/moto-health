using System.Threading.Tasks;
using MotoHealth.Functions.ChatSubscriptions;

namespace MotoHealth.Functions.AccidentAlerting
{
    public interface IAccidentAlertingSubscriptionsService
    {
        Task<ChatSubscription[]> GetAllChatSubscriptionsAsync();
    }

    internal sealed class AccidentAlertingSubscriptionsService : IAccidentAlertingSubscriptionsService
    {
        private const string SubscriptionTopic = "Accidents";

        private readonly IChatSubscriptionsEventsStore _eventsStore;

        public AccidentAlertingSubscriptionsService(IChatSubscriptionsEventsStore eventsStore)
        {
            _eventsStore = eventsStore;
        }

        public async Task<ChatSubscription[]> GetAllChatSubscriptionsAsync()
        {
            return await _eventsStore.GetTopicSubscriptionsAsync(SubscriptionTopic);
        }
    }
}