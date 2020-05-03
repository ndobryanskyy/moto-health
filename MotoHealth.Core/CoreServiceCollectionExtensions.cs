using System;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
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
                .AddSingleton<IBotUpdatesMapper, BotUpdatesMapper>()
                .AddSingleton<ITelegramBotClientFactory, TelegramBotClientFactory>()
                .AddTransient<IChatsFactory, ChatsFactory>()
                .AddTransient<IChatStatesRepository, ChatStatesRepository>()
                .AddSingleton<IChatsDoorman, ChatsDoorman>()
                .AddSingleton<IBotCommandsRegistry, BotCommandsRegistry>()
                .AddTransient<IChatUpdateHandler, MainChatUpdateHandler>()
                .AddTransient<IAccidentReportDialogHandler, AccidentReportDialogHandler>();

            return services;
        }
    }
}