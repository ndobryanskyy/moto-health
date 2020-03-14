using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Bot.Messages;
using MotoHealth.Bot.ServiceBus;
using MotoHealth.Bot.Telegram;

namespace MotoHealth.Bot.Extensions.Startup
{
    internal static class UpdatesQueueServiceCollectionExtensions
    {
        public static IServiceCollection AddUpdatesQueue(this IServiceCollection services)
        {
            services
                .AddSingleton<IQueueClientsFactory, QueueClientsFactory>()
                .AddSingleton<IBotUpdateResolver, BotUpdateResolver>()
                .AddSingleton<IBotUpdateSerializer, BotUpdateSerializer>()
                .AddSingleton<ITelegramUpdatesQueue, TelegramUpdatesQueue>();

            services.AddHostedService<UpdatesQueueHandlerBackgroundService>();

            return services;
        }
    }
}