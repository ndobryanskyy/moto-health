using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Infrastructure.AzureEventGrid;
using MotoHealth.PubSub;
using MotoHealth.PubSub.EventData;

namespace MotoHealth.Infrastructure.ChatSubscriptions
{
    internal sealed class AzureEventGridChatSubscriptionsService : IChatSubscriptionsService
    {
        private readonly ILogger<AzureEventGridChatSubscriptionsService> _logger;
        private readonly IAzureEventGridPublisher _eventGridPublisher;

        public AzureEventGridChatSubscriptionsService(
            ILogger<AzureEventGridChatSubscriptionsService> logger,
            IAzureEventGridPublisher eventGridPublisher)
        {
            _logger = logger;
            _eventGridPublisher = eventGridPublisher;
        }

        public async Task SubscribeChatToTopicAsync(long chatId, string topic, CancellationToken cancellationToken)
        {
            await PublishChangesAsync(chatId, topic, true, cancellationToken);
        }

        public async Task UnsubscribeChatFromTopicAsync(long chatId, string topic, CancellationToken cancellationToken)
        {
            await PublishChangesAsync(chatId, topic, false, cancellationToken);
        }

        private async Task PublishChangesAsync(
            long chatId, 
            string topic, 
            bool subscribed,
            CancellationToken cancellationToken)
        {
            var eventId = Guid.NewGuid().ToString();

            var eventData = new ChatTopicSubscriptionChangedEventData
            {
                ChatId = chatId,
                Topic = topic,
                IsSubscribed = subscribed
            };

            var eventGridEvent = new EventGridEvent
            {
                Id = eventId,
                Subject = chatId.ToString(),
                EventType = EventTypes.ChatTopicSubscriptionChanged,
                EventTime = DateTime.UtcNow,
                Data = eventData,
                DataVersion = ChatTopicSubscriptionChangedEventData.Version
            };

            _logger.LogInformation($"Started publishing chat subscription changes to {subscribed} for {chatId}");

            await _eventGridPublisher.PublishEventAsync(eventGridEvent, cancellationToken);

            _logger.LogInformation($"Finished publishing chat subscription changes to {subscribed} for {chatId}");
        }
    }
}