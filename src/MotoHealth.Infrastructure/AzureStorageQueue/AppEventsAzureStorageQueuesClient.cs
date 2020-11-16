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
    public interface IAppEventsQueuesClient
    {
        Task InitializeQueuesAsync(CancellationToken cancellationToken);

        Task PublishAccidentAlertAsync(AccidentAlertDto alert, CancellationToken cancellationToken);
    }

    internal interface IAppQueuesStatusProvider
    {
        Task<bool> CheckQueuesExistAsync(CancellationToken cancellationToken);
    }

    internal sealed class AppEventsAzureStorageQueuesClient : IAppEventsQueuesClient, IAppQueuesStatusProvider
    {
        private readonly ILogger<AppEventsAzureStorageQueuesClient> _logger;
        private readonly QueueClient _alertsQueueClient;

        public AppEventsAzureStorageQueuesClient(
            HttpClient client,
            ILogger<AppEventsAzureStorageQueuesClient> logger,
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

            await _alertsQueueClient.SendMessageAsync(encoded, timeToLive: TimeSpan.FromDays(3), cancellationToken: cancellationToken);

            _logger.LogDebug($"Successfully added {alert.Report.Id} to queue");
        }

        public async Task InitializeQueuesAsync(CancellationToken cancellationToken)
        {
            await _alertsQueueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            _logger.LogDebug("Alerts queue initialized");
        }

        async Task<bool> IAppQueuesStatusProvider.CheckQueuesExistAsync(CancellationToken cancellationToken) 
            => await _alertsQueueClient.ExistsAsync(cancellationToken);
    }
}