using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Bot.Telegram;

namespace MotoHealth.Bot
{
    internal static class AppServiceCollectionExtensions
    {
        public static IServiceCollection AddApp(this IServiceCollection services)
        {
            services
                .AddTransient<IBotUpdateResolver, BotUpdateResolver>();
                
            services
                .AddHostedService<BotInitializerStartupJob>();

            return services;
        }
    }
}