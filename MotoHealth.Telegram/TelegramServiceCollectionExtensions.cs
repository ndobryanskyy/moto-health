using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace MotoHealth.Telegram
{
    public static class TelegramServiceCollectionExtensions
    {
        public static IServiceCollection AddTelegram(this IServiceCollection services)
        {
            return AddCoreServices(services);
        }

        public static IServiceCollection AddTelegram(
            this IServiceCollection services,
            Action<TelegramOptions> configureTelegramOptions)
        {
            return AddCoreServices(services)
                .Configure(configureTelegramOptions);
        }

        private static IServiceCollection AddCoreServices(IServiceCollection services)
        {
            return services
                .AddSingleton<ITelegramBotClientFactory, TelegramBotClientFactory>()
                .AddSingleton(CreateSharedTelegramClient);
        }

        private static ITelegramBotClient CreateSharedTelegramClient(IServiceProvider services)
        {
            var factory = services.GetRequiredService<ITelegramBotClientFactory>();
            var options = services.GetRequiredService<IOptions<TelegramOptions>>();

            return factory.CreateClient(options.Value);
        }
    }
}