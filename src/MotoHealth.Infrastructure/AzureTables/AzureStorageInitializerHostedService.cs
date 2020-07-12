using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MotoHealth.Infrastructure.AzureTables
{
    internal sealed class AzureStorageInitializerHostedService : IHostedService
    {
        private readonly ILogger<AzureStorageInitializerHostedService> _logger;
        private readonly IAzureStorageInitializer _initializer;

        public AzureStorageInitializerHostedService(
            ILogger<AzureStorageInitializerHostedService> logger,
            IAzureStorageInitializer initializer)
        {
            _logger = logger;
            _initializer = initializer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Azure storage initialization started");

            await _initializer.InitializeAsync(cancellationToken);

            _logger.LogInformation("Azure storage initialization finished");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Does nothing

            return Task.CompletedTask;
        }
    }
}