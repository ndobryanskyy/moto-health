using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MotoHealth.Infrastructure.AzureStorageQueue
{
    internal sealed class AzureStorageQueuesHealthCheck : IHealthCheck
    {
        private readonly IAppQueuesStatusProvider _queuesStatusProvider;

        public AzureStorageQueuesHealthCheck(IAppQueuesStatusProvider queuesStatusProvider)
        {
            _queuesStatusProvider = queuesStatusProvider;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            try
            {
                var queuesExist = await _queuesStatusProvider.CheckQueuesExistAsync(cancellationToken);

                if (!queuesExist)
                {
                    return new HealthCheckResult(context.Registration.FailureStatus, "App Queues do not exist");
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception exception)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: exception);
            }
        }
    }
}