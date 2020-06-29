using System;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Core.Bot.ChatUpdateHandlers;
using MotoHealth.Core.Bot.Commands;
using MotoHealth.Core.Telegram;
using MotoHealth.Telegram;

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

            services.AddTelegram(configurator.ConfigureTelegram);

            services
                .AddSingleton<IBotUpdatesMapper, BotUpdatesMapper>()
                .AddTransient<IChatFactory, ChatFactory>()
                .AddTransient<IChatStatesRepository, ChatStatesRepository>()
                .AddSingleton<IChatsDoorman, ChatsDoorman>()
                .AddSingleton<IBotCommandsRegistry, BotCommandsRegistry>()
                .AddSingleton<IPhoneNumberParser, PhoneNumberParser>();

            services
                .AddTransient<BannedUsersChatUpdateHandler>();

            services
                .AddTransient<AdminCommandsChatUpdateHandler>()
                .AddSingleton<IAdminCommandsChatUpdateHandlerMessages, AdminCommandsChatUpdateHandlerMessages>();

            services
                .AddTransient<AccidentReportingDialogChatUpdateHandler>()
                .AddTransient<IAccidentReportingDialogHandler, AccidentReportingDialogHandler>()
                .AddSingleton<IAccidentReportingDialogMessages, AccidentReportingDialogMessages>();

            services
                .AddTransient<MainChatUpdateHandler>()
                .AddSingleton<IMainChatUpdateHandlerMessages, MainChatUpdateHandlerMessages>();

            return services;
        }
    }
}