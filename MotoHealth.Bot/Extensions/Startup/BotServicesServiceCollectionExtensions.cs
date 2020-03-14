using Microsoft.Extensions.DependencyInjection;

namespace MotoHealth.Bot.Extensions.Startup
{
    internal static class BotServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddBots(this IServiceCollection services)
        {
            services
                .AddTransient<IBotsRepository, BotsRepository>()
                .AddSingleton<IBotsInMemoryCache, BotsInMemoryCache>()
                .AddSingleton<IBotFactory, BotFactory>()
                .AddSingleton<IBotContextFactory, BotContextFactory>();

            return services;
        }
    }
}