using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MotoHealth.Telegram;

namespace MotoHealth.Bot.Telegram.HealthChecks
{
    internal sealed class TelegramHealthCheck : IHealthCheck
    {
        private readonly ITelegramClient _telegramClient;

        public TelegramHealthCheck(ITelegramClient telegramClient)
        {
            _telegramClient = telegramClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            try
            {
                await _telegramClient.GetMeAsync(cancellationToken);

                return HealthCheckResult.Healthy();
            }
            catch (Exception exception)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: exception);
            }
        }
    }
}