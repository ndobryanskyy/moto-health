using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MotoHealth.Infrastructure.AzureTables
{
    internal static class AzureTablesHealthChecksExtensions
    {
        private const string HealthCheckName = "Azure Tables";

        public static IHealthChecksBuilder AddAzureTables(this IHealthChecksBuilder builder)
        {
            builder.Services.AddTransient<AzureTablesHealthCheck>();

            var registration = new HealthCheckRegistration(
                HealthCheckName,
                CreateHealthCheck,
                HealthStatus.Degraded,
                Array.Empty<string>());

            return builder.Add(registration);
        }

        private static IHealthCheck CreateHealthCheck(IServiceProvider container)
            => container.GetRequiredService<AzureTablesHealthCheck>();
    }
}