using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Options;

namespace MotoHealth.Infrastructure.AzureEventGrid
{
    public interface IAzureEventGridPublisher
    {
        Task PublishEventAsync(EventGridEvent eventGridEvent, CancellationToken cancellationToken);
    }

    internal sealed class AzureEventGridPublisher : IAzureEventGridPublisher
    {
        private readonly EventGridClient _client;
        private readonly string _hostname;

        public AzureEventGridPublisher(IOptions<AzureEventGridOptions> options)
        {
            var eventGridOptions = options.Value;
            
            var topicCredentials = new TopicCredentials(eventGridOptions.TopicKey);

            _client = new EventGridClient(topicCredentials);
            _hostname = new Uri(options.Value.TopicEndpoint).Host;
        }

        public async Task PublishEventAsync(EventGridEvent eventGridEvent, CancellationToken cancellationToken)
        {
            var events = new List<EventGridEvent> {eventGridEvent};
            await _client.PublishEventsAsync(_hostname, events, cancellationToken);
        }
    }
}