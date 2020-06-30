using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.Common;
using MotoHealth.Common.Dto;

namespace MotoHealth.Infrastructure.AzureEventGrid
{
    public interface IAppEventsTopicClient
    {
        Task PublishAccidentAlertAsync(AccidentAlertEventDataDto eventData, CancellationToken cancellationToken);
    }

    internal sealed class AppEventsTopicClient : IAppEventsTopicClient, IDisposable
    {
        private readonly ILogger<AppEventsTopicClient> _logger;
        private readonly EventGridClient _eventGridClient;
        private readonly string _topicHostname;

        public AppEventsTopicClient(
            HttpClient httpClient,
            ILogger<AppEventsTopicClient> logger,
            IOptions<InfrastructureOptions> options)
        {
            _logger = logger;

            var topicOptions = options.Value.AppEventsTopic;
            var topicCredentials = new TopicCredentials(topicOptions.TopicKey);
            _topicHostname = new Uri(topicOptions.TopicEndpoint).Host;

            _eventGridClient = new EventGridClient(topicCredentials, httpClient, true);
        }

        public async Task PublishAccidentAlertAsync(AccidentAlertEventDataDto eventData, CancellationToken cancellationToken)
        {
            var reportId = eventData.Report.Id;

            var gridEvent = new EventGridEvent
            {
                Id = Guid.NewGuid().ToString(),
                Subject = reportId,
                EventType = CommonConstants.EventTypes.AccidentAlerted,
                EventTime = DateTime.UtcNow,
                Data = eventData,
                DataVersion = AccidentAlertEventDataDto.Version
            };

            await _eventGridClient.PublishEventsAsync(_topicHostname, new [] { gridEvent }, cancellationToken);

            _logger.LogInformation($"Successfully published event {gridEvent.Id} for report {reportId}");
        }

        public void Dispose()
        {
            _eventGridClient.Dispose();
        }
    }
}