﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using Microsoft.Extensions.Configuration;
using MotoHealth.Functions.Extensions;

namespace MotoHealth.Functions.AdminBot.ChatSubscriptions
{
    public interface IChatSubscriptionsManager
    {
        Task SubscribeChatToTopicAsync(long chatId, string topic);

        Task UnsubscribeChatFromTopicAsync(long chatId, string topic);

        Task<bool> CheckIfChatIsSubscribedToTopicAsync(long chatId, string topic);

        Task<IReadOnlyCollection<IChatSubscription>> GetTopicSubscriptions(string topic);
    }

    public sealed class ChatSubscriptionsManager : IChatSubscriptionsManager
    {
        private const string TableName = "ChatSubscriptions";
        
        private readonly CloudTable _tableClient;

        private bool _isTableInitialized;

        public ChatSubscriptionsManager(IConfiguration configuration)
        {
            var storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString("StorageAccount"));
            var client = storageAccount.CreateCloudTableClient();

            _tableClient = client.GetTableReference(TableName);
        }

        public async Task SubscribeChatToTopicAsync(long chatId, string topic)
        {
            await EnsureTableExistsAsync();

            var subscription = new ChatSubscriptionTableEntity
            {
                ChatId = chatId,
                Topic = topic,
                IsEnabled = true
            };

            var operation = TableOperation.InsertOrMerge(subscription);

            await _tableClient.ExecuteAsync(operation);
        }

        public async Task UnsubscribeChatFromTopicAsync(long chatId, string topic)
        {
            await EnsureTableExistsAsync();

            var subscription = new ChatSubscriptionTableEntity
            {
                ChatId = chatId,
                Topic = topic,
                IsEnabled = false,
                ETag = "*"
            };

            var operation = TableOperation.Merge(subscription);

            await _tableClient.ExecuteAsync(operation);
        }

        public async Task<bool> CheckIfChatIsSubscribedToTopicAsync(long chatId, string topic)
        {
            await EnsureTableExistsAsync();

            var operation = TableOperation.Retrieve<ChatSubscriptionTableEntity>(topic, chatId.ToString());

            var operationResult = await _tableClient.ExecuteAsync(operation);

            if (operationResult.Result is ChatSubscriptionTableEntity subscription)
            {
                return subscription.IsEnabled;
            }

            return false;
        }

        public async Task<IReadOnlyCollection<IChatSubscription>> GetTopicSubscriptions(string topic)
        {
            await EnsureTableExistsAsync();

            var subscriptions = await _tableClient.CreateQuery<ChatSubscriptionTableEntity>()
                .Where(x => x.PartitionKey == topic && x.IsEnabled)
                .AsTableQuery()
                .ToListAsync();

            return subscriptions;
        }

        private async ValueTask EnsureTableExistsAsync()
        {
            if (_isTableInitialized) return;

            await _tableClient.CreateIfNotExistsAsync();
            
            _isTableInitialized = true;
        }
    }
}