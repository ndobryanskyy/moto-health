using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MotoHealth.Bot.Telegram.HealthChecks
{
    internal static class TelegramHealthChecksExtensions
    {
        private const string HealthCheckName = "Telegram";

        public static IHealthChecksBuilder AddTelegram(this IHealthChecksBuilder builder)
        {
            builder.Services.AddTransient<TelegramHealthCheck>();

            var registration = new HealthCheckRegistration(
                HealthCheckName,
                CreateHealthCheck,
                HealthStatus.Unhealthy,
                Array.Empty<string>());

            return builder.Add(registration);
        }

        private static IHealthCheck CreateHealthCheck(IServiceProvider container)
            => container.GetRequiredService<TelegramHealthCheck>();
    }
}