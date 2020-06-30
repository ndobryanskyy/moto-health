using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MotoHealth.Infrastructure.AzureTables
{
    internal sealed class AzureTablesInitializerHostedService : IHostedService
    {
        private readonly ILogger<AzureTablesInitializerHostedService> _logger;
        private readonly IAzureTablesInitializer _initializer;

        public AzureTablesInitializerHostedService(
            ILogger<AzureTablesInitializerHostedService> logger,
            IAzureTablesInitializer initializer)
        {
            _logger = logger;
            _initializer = initializer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Azure tables initialization started");

            await _initializer.InitializeAllAsync(cancellationToken);

            _logger.LogInformation("Azure tables initialization finished");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Does nothing

            return Task.CompletedTask;
        }
    }
}