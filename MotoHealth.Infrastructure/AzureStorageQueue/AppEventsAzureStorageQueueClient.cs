using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Azure.Storage.Queues;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.Common;
using MotoHealth.Events.Dto;

namespace MotoHealth.Infrastructure.AzureStorageQueue
{
    public interface IAppEventsQueueClient
    {
        Task PublishAccidentAlertAsync(AccidentAlertDto alert, CancellationToken cancellationToken);
    }

    internal sealed class AppEventsAzureStorageQueueClient : IAppEventsQueueClient
    {
        private readonly ILogger<AppEventsAzureStorageQueueClient> _logger;
        private readonly QueueClient _alertsQueueClient;

        public AppEventsAzureStorageQueueClient(
            HttpClient client,
            ILogger<AppEventsAzureStorageQueueClient> logger,
            IOptions<InfrastructureOptions> options)
        {
            _logger = logger;

            _alertsQueueClient = new QueueClient(
                options.Value.AzureStorage.StorageAccountConnectionString, 
                CommonConstants.AccidentReporting.AlertsQueueName,
                new QueueClientOptions
                {
                    Transport = new HttpClientTransport(client)
                });
        }

        public async Task PublishAccidentAlertAsync(AccidentAlertDto alert, CancellationToken cancellationToken)
        {
            var bytes = alert.ToByteArray();
            var encoded = Convert.ToBase64String(bytes);

            await _alertsQueueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            await _alertsQueueClient.SendMessageAsync(encoded, timeToLive: TimeSpan.FromDays(3), cancellationToken: cancellationToken);

            _logger.LogDebug($"Successfully added {alert.Report.Id} to queue");
        }
    }
}