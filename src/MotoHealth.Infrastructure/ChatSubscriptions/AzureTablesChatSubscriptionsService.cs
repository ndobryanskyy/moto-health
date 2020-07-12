using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Infrastructure.AzureTables;

namespace MotoHealth.Infrastructure.ChatSubscriptions
{
    internal sealed class AzureTablesChatSubscriptionsService : IChatSubscriptionsService
    {
        private readonly ILogger<AzureTablesChatSubscriptionsService> _logger;
        private readonly CloudTable _subscriptionsTable;

        public AzureTablesChatSubscriptionsService(
            ILogger<AzureTablesChatSubscriptionsService> logger,
            ICloudTablesProvider cloudTablesProvider)
        {
            _logger = logger;
            _subscriptionsTable = cloudTablesProvider.ChatSubscriptions;
        }

        public async Task SubscribeChatToTopicAsync(long chatId, string topic, CancellationToken cancellationToken)
        {
            var subscription = new ChatSubscriptionTableEntity
            {
                ChatId = chatId,
                Topic = topic
            };

            var operation = TableOperation.InsertOrReplace(subscription);

            await _subscriptionsTable.ExecuteAsync(operation, cancellationToken);

            _logger.LogInformation($"Chat {chatId} subscribed to topic '{topic}'");
        }

        public async Task UnsubscribeChatFromTopicAsync(long chatId, string topic, CancellationToken cancellationToken)
        {
            var retrieveOperation = TableOperation.Retrieve<ChatSubscriptionTableEntity>(
                topic,
                chatId.ToString(CultureInfo.InvariantCulture)
            );

            var result = await _subscriptionsTable.ExecuteAsync(retrieveOperation, cancellationToken);

            if (result.Result is ChatSubscriptionTableEntity subscription)
            {
                _logger.LogDebug($"Subscription on '{topic}' topic for chat {chatId} found");

                await _subscriptionsTable.ExecuteAsync(TableOperation.Delete(subscription), cancellationToken);

                _logger.LogInformation($"Chat {chatId} unsubscribed from topic '{topic}'");
            }
            else
            {
                _logger.LogWarning($"Tried to unsubscribe chat {chatId} which was not subscribed to topic '{topic}'");
            }
        }

        public async Task<IReadOnlyList<IChatSubscription>> GetTopicSubscriptionsAsync(string topic, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Getting chat subscriptions for topic '{topic}'");

            return await _subscriptionsTable.CreateQuery<ChatSubscriptionTableEntity>()
                .AsQueryable()
                .Where(x => x.PartitionKey == topic)
                .AsTableQuery()
                .ToListAsync(cancellationToken);
        }
    }
}