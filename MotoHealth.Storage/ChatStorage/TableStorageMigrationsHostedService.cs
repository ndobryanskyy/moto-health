using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MotoHealth.Infrastructure.ChatStorage
{
    internal sealed class TableStorageMigrationsHostedService : IHostedService
    {
        private readonly ILogger<TableStorageMigrationsHostedService> _logger;
        private readonly ICloudTablesProvider _tablesProvider;

        public TableStorageMigrationsHostedService(
            ILogger<TableStorageMigrationsHostedService> logger,
            ICloudTablesProvider tablesProvider)
        {
            _logger = logger;
            _tablesProvider = tablesProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting migration");

            await _tablesProvider.Chats.CreateIfNotExistsAsync(cancellationToken);

            _logger.LogInformation("Migration finished successfully");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Does nothing

            return Task.CompletedTask;
        }
    }
}