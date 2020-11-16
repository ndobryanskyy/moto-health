using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using MotoHealth.Infrastructure.AzureStorageQueue;

namespace MotoHealth.Infrastructure.AzureTables
{
    internal interface IAzureStorageInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken);
    }

    internal sealed class AzureStorageInitializer : IAzureStorageInitializer
    {
        private readonly ILogger<AzureStorageInitializer> _logger;
        private readonly ICloudTablesProvider _tablesProvider;
        private readonly IAppEventsQueuesClient _queuesClient;

        public AzureStorageInitializer(
            ILogger<AzureStorageInitializer> logger,
            ICloudTablesProvider tablesProvider,
            IAppEventsQueuesClient queuesClient)
        {
            _logger = logger;
            _tablesProvider = tablesProvider;
            _queuesClient = queuesClient;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _queuesClient.InitializeQueuesAsync(cancellationToken);

                await EnsureTableExistsAsync(_tablesProvider.Chats, cancellationToken);
                await EnsureTableExistsAsync(_tablesProvider.ChatSubscriptions, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to initialize storage");
                
                throw;
            }
        }

        private async Task EnsureTableExistsAsync(CloudTable table, CancellationToken cancellationToken)
        {
            var tableCreated = await table.CreateIfNotExistsAsync(cancellationToken);

            if (tableCreated)
            {
                _logger.LogInformation($"{table.Name} created.");
            }
            else
            {
                _logger.LogDebug($"{table.Name} already exists.");
            }
        }
    }
}