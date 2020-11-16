using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MotoHealth.Infrastructure.AzureStorageQueue
{
    internal static class AzureStorageQueuesHealthChecksExtensions
    {
        private const string HealthCheckName = "Azure Storage Queues";

        public static IHealthChecksBuilder AddAzureStorageQueues(this IHealthChecksBuilder builder)
        {
            builder.Services.AddTransient<AzureStorageQueuesHealthCheck>();

            var registration = new HealthCheckRegistration(
                HealthCheckName,
                CreateHealthCheck,
                HealthStatus.Degraded,
                Array.Empty<string>());

            return builder.Add(registration);
        }

        private static IHealthCheck CreateHealthCheck(IServiceProvider container)
            => container.GetRequiredService<AzureStorageQueuesHealthCheck>();
    }
}