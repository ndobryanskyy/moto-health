using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using MotoHealth.PubSub;
using MotoHealth.PubSub.EventData;

namespace MotoHealth.Functions.ChatSubscriptions
{
    public sealed class ChatTopicSubscriptionChangedEventHandler
    {
        private readonly ILogger<ChatTopicSubscriptionChangedEventHandler> _logger;
        private readonly IEventGridEventDataParser _dataParser;
        private readonly IChatSubscriptionsEventsStore _eventsStore;

        public ChatTopicSubscriptionChangedEventHandler(
            ILogger<ChatTopicSubscriptionChangedEventHandler> logger,
            IEventGridEventDataParser dataParser,
            IChatSubscriptionsEventsStore eventsStore)
        {
            _logger = logger;
            _dataParser = dataParser;
            _eventsStore = eventsStore;
        }

        [FunctionName(FunctionNames.ChatTopicSubscriptionChangedEventHandler)]
        public async Task HandleEventAsync([EventGridTrigger] EventGridEvent eventGridEvent)
        {
            if (eventGridEvent.EventType != EventTypes.ChatTopicSubscriptionChanged)
            {
                _logger.LogError($"Got event {eventGridEvent.Id} of wrong type: {eventGridEvent.EventType}");
                return;
            }

            _logger.LogInformation($"Start handling {eventGridEvent.Id}");

            if (_dataParser.TryParseEventData<ChatTopicSubscriptionChangedEventData>(eventGridEvent, out var eventData))
            {
                await _eventsStore.StoreEventAsync(eventGridEvent.Id, eventGridEvent.EventTime, eventData);

                _logger.LogInformation($"Successfully finished handling {eventGridEvent.Id}");
            }
            else
            {
                _logger.LogError($"Failed to handle {eventGridEvent.Id}");
            }
        }
    }
}