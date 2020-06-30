using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace MotoHealth.Infrastructure.AzureTables
{
    internal interface IAzureTablesInitializer
    {
        Task InitializeAllAsync(CancellationToken cancellationToken);
    }

    internal sealed class AzureTablesInitializer : IAzureTablesInitializer
    {
        private readonly ILogger<AzureTablesInitializer> _logger;
        private readonly ICloudTablesProvider _tablesProvider;

        public AzureTablesInitializer(
            ILogger<AzureTablesInitializer> logger,
            ICloudTablesProvider tablesProvider)
        {
            _logger = logger;
            _tablesProvider = tablesProvider;
        }

        public async Task InitializeAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                await EnsureTableExistsAsync(_tablesProvider.Chats, cancellationToken);
                await EnsureTableExistsAsync(_tablesProvider.ChatSubscriptions, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to initialize tables");
                
                throw new Exception("Fatal startup error");
            }
        }

        private async Task EnsureTableExistsAsync(CloudTable table, CancellationToken cancellationToken)
        {
            var chatsTableCreated = await table.CreateIfNotExistsAsync(cancellationToken);

            if (chatsTableCreated)
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