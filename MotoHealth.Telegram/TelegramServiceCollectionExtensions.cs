using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace MotoHealth.Telegram
{
    public static class TelegramServiceCollectionExtensions
    {
        private static readonly Random Jitter = new Random();

        private static readonly IAsyncPolicy<HttpResponseMessage> ClientRetryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(result => result.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(2, attempt =>
            {
                var milliseconds = (attempt * 500) + Jitter.Next(0, 200);
                return TimeSpan.FromMilliseconds(milliseconds);
            });

        public static IServiceCollection AddTelegram(this IServiceCollection services) 
            => AddCoreServices(services);

        public static IServiceCollection AddTelegram(
            this IServiceCollection services,
            Action<TelegramClientOptions> configureTelegramOptions)
        {
            return AddCoreServices(services)
                .Configure(configureTelegramOptions);
        }

        private static IServiceCollection AddCoreServices(IServiceCollection services)
        {
            services
                .AddHttpClient<ITelegramClient, TelegramClient>((container, client) =>
                {
                    var telegramOptions = container.GetRequiredService<IOptions<TelegramClientOptions>>().Value;

                    client.BaseAddress = telegramOptions.BaseAddress;
                    client.Timeout = telegramOptions.RequestTimeout;
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(10))
                .AddPolicyHandler(ClientRetryPolicy);

            return services;
        }
    }
}