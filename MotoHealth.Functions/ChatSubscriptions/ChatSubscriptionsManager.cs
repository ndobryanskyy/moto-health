using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using MotoHealth.Functions.Extensions;
using MotoHealth.Telegram.Extensions;
using Telegram.Bot.Types;

namespace MotoHealth.Functions.ChatSubscriptions
{
    public interface IChatSubscriptionsManager
    {
        Task SubscribeChatToTopicAsync(Chat chat, string topic);

        Task UnsubscribeChatFromTopicAsync(Chat chat, string topic);

        Task<bool> CheckIfChatIsSubscribedToTopicAsync(Chat chat, string topic);

        Task<ChatSubscription[]> GetTopicSubscriptions(string topic);
    }

    public sealed class ChatSubscriptionsManager : IChatSubscriptionsManager
    {
        private readonly CloudTable _subscriptionsTable;

        private bool _isTableInitialized;

        public ChatSubscriptionsManager(ICloudTablesProvider tables)
        {
            _subscriptionsTable = tables.ChatSubscriptions;
        }

        public async Task SubscribeChatToTopicAsync(Chat chat, string topic)
        {
            await EnsureTableExistsAsync();

            var subscription = new ChatSubscriptionTableEntity
            {
                ChatId = chat.Id,
                ChatName = chat.GetFriendlyName(),
                Topic = topic,
                IsEnabled = true
            };

            var operation = TableOperation.InsertOrMerge(subscription);

            await _subscriptionsTable.ExecuteAsync(operation);
        }

        public async Task UnsubscribeChatFromTopicAsync(Chat chat, string topic)
        {
            await EnsureTableExistsAsync();

            var subscription = new ChatSubscriptionTableEntity
            {
                ChatId = chat.Id,
                Topic = topic,
                IsEnabled = false,
                ETag = "*"
            };

            var operation = TableOperation.Merge(subscription);

            await _subscriptionsTable.ExecuteAsync(operation);
        }

        public async Task<bool> CheckIfChatIsSubscribedToTopicAsync(Chat chat, string topic)
        {
            await EnsureTableExistsAsync();

            var operation = TableOperation.Retrieve<ChatSubscriptionTableEntity>(topic, chat.Id.ToString());

            var operationResult = await _subscriptionsTable.ExecuteAsync(operation);

            if (operationResult.Result is ChatSubscriptionTableEntity subscription)
            {
                return subscription.IsEnabled;
            }

            return false;
        }

        public async Task<ChatSubscription[]> GetTopicSubscriptions(string topic)
        {
            await EnsureTableExistsAsync();

            var subscriptions = await _subscriptionsTable.CreateQuery<ChatSubscriptionTableEntity>()
                .Where(x => x.PartitionKey == topic && x.IsEnabled)
                .Select(x => x.ChatId)
                .AsTableQuery()
                .ToListAsync();

            return subscriptions
                .Select(x => new ChatSubscription(x))
                .ToArray();
        }

        private async ValueTask EnsureTableExistsAsync()
        {
            if (_isTableInitialized) return;

            await _subscriptionsTable.CreateIfNotExistsAsync();
            
            _isTableInitialized = true;
        }
    }
}