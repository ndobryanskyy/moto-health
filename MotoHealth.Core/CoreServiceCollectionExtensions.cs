using System;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Messages;
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
                .AddTransient<IChatStatesRepository, ChatStatesRepository>()
                .AddSingleton<ITelegramBotClientFactory, TelegramBotClientFactory>()
                .AddSingleton<IChatFactory, ChatFactory>()
                .AddSingleton<IMessageFactory, MessageFactory>()
                .AddTransient<IBotUpdateHandler, BotUpdateHandler>()
                .AddTransient<IAccidentReportDialogHandler, AccidentReportDialogHandler>();

            return services;
        }
    }
}