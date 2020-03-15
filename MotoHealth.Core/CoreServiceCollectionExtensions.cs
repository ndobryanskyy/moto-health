using System;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Telegram;

namespace MotoHealth.Core
{
    public static class CoreServiceCollectionExtensions
    {
        public static IServiceCollection AddCore(
            this IServiceCollection services,
            CoreOptionsConfigurator configurator)
        {
            if (configurator.ConfigureTelegram == null)
            {
                throw new InvalidOperationException($"{nameof(CoreOptionsConfigurator.ConfigureTelegram)} must be set");
            }

            services.Configure(configurator.ConfigureTelegram);

            services
                .AddTransient<IChatControllersRepository, ChatControllersRepository>()
                .AddSingleton<ITelegramBotClientFactory, TelegramBotClientFactory>()
                .AddSingleton<IChatControllersFactory, ChatControllersFactory>()
                .AddSingleton<IBotUpdateContextFactory, BotUpdateContextFactory>()
                .AddTransient<IBotUpdatesHandler, BotUpdatesHandler>();

            return services;
        }
    }
}