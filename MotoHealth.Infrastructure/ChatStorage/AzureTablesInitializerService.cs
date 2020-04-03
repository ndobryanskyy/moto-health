using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace MotoHealth.Infrastructure.ChatStorage
{
    internal interface IAzureTablesInitializer
    {
        Task InitializeAllAsync(CancellationToken cancellationToken);
    }

    internal sealed class AzureTablesInitializer : IAzureTablesInitializer
    {
        private readonly ILogger<AzureTablesInitializer> _logger;
        private readonly CloudTable _chatsTable;

        public AzureTablesInitializer(
            ILogger<AzureTablesInitializer> logger,
            ICloudTablesProvider tablesProvider)
        {
            _logger = logger;
            _chatsTable = tablesProvider.Chats;
        }

        public async Task InitializeAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                await CreateChatsTableAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to initialize tables");
                
                throw new Exception("Fatal startup error");
            }
        }

        private async Task CreateChatsTableAsync(CancellationToken cancellationToken)
        {
            var chatsTableCreated = await _chatsTable.CreateIfNotExistsAsync(cancellationToken);

            if (chatsTableCreated)
            {
                _logger.LogInformation("Chats table created.");
            }
            else
            {
                _logger.LogDebug("Chats table already exists.");
            }
        }
    }
}