using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MotoHealth.Infrastructure.AzureTables
{
    internal sealed class AzureTablesHealthCheck : IHealthCheck
    {
        private readonly ICloudTablesProvider _tables;

        public AzureTablesHealthCheck(ICloudTablesProvider tables)
        {
            _tables = tables;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                var chatsTableExists = await _tables.Chats.ExistsAsync(cancellationToken);
                if (!chatsTableExists)
                {
                    return new HealthCheckResult(context.Registration.FailureStatus, "Azure Table 'Chats' does not exist");
                }

                var chatSubscriptionsTableExists = await _tables.ChatSubscriptions.ExistsAsync(cancellationToken);
                if (!chatSubscriptionsTableExists)
                {
                    return new HealthCheckResult(context.Registration.FailureStatus, "Azure Table 'ChatSubscriptions' does not exist");
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