using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Protocol;
using Microsoft.Azure.Cosmos.Table.Queryable;
using Microsoft.Extensions.Logging;
using MotoHealth.Functions.Extensions;
using MotoHealth.PubSub.EventData;

namespace MotoHealth.Functions.ChatSubscriptions
{
    public interface IChatSubscriptionsEventsStore
    {
        Task StoreEventAsync(string eventId, DateTime eventTime, ChatTopicSubscriptionChangedEventData eventData);

        Task<ChatSubscription[]> GetTopicSubscriptionsAsync(string topic);
    }

    public sealed class ChatSubscriptionsEventsStore : IChatSubscriptionsEventsStore
    {
        private readonly ILogger<ChatSubscriptionsEventsStore> _logger;
        private readonly CloudTable _eventsTable;

        private bool _isTableInitialized;

        public ChatSubscriptionsEventsStore(
            ICloudTablesProvider tables,
            ILogger<ChatSubscriptionsEventsStore> logger)
        {
            _logger = logger;
            _eventsTable = tables.ChatSubscriptionsEvents;
        }

        public async Task StoreEventAsync(string eventId, DateTime eventTime, ChatTopicSubscriptionChangedEventData eventData)
        {
            await EnsureTableExistsAsync();

            var eventEntity = new ChatSubscriptionEventTableEntity
            {
                EventId = eventId,
                EventTime = eventTime,
                ChatId = eventData.ChatId,
                Topic = eventData.Topic,
                IsSubscribed = eventData.IsSubscribed
            };

            var operation = TableOperation.Insert(eventEntity);

            _logger.LogInformation($"Started storing event {eventId}");

            try
            {
                await _eventsTable.ExecuteAsync(operation);

                _logger.LogInformation($"Successfully finished storing event {eventId}");
            }
            catch (StorageException exception) when (exception.RequestInformation.ExtendedErrorInformation.ErrorCode == TableErrorCodeStrings.EntityAlreadyExists)
            {
                _logger.LogWarning($"Ignored event {eventId} duplicate");
            }
        }

        public async Task<ChatSubscription[]> GetTopicSubscriptionsAsync(string topic)
        {
            await EnsureTableExistsAsync();

            var subscriptions = await _eventsTable.CreateQuery<ChatSubscriptionEventTableEntity>()
                .Where(x => x.PartitionKey == topic)
                .AsTableQuery()
                .ToListAsync();

            return subscriptions
                .GroupBy(x => x.ChatId)
                .Select(group => group.OrderByDescending(x => x.EventTime).First())
                .Where(x => x.IsSubscribed)
                .Select(x => new ChatSubscription(x.ChatId))
                .ToArray();
        }

        private async ValueTask EnsureTableExistsAsync()
        {
            if (_isTableInitialized) return;

            await _eventsTable.CreateIfNotExistsAsync();
            
            _isTableInitialized = true;
        }
    }
}