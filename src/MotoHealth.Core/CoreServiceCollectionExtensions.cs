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
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddTelegram();

            services
                .AddSingleton<IBotUpdatesMapper, BotUpdatesMapper>()
                .AddScoped<IChatFactory, ChatFactory>()
                .AddTransient<IChatStatesRepository, ChatStatesRepository>()
                .AddSingleton<IChatsDoorman, ChatsDoorman>()
                .AddSingleton<IBotCommandsRegistry, BotCommandsRegistry>()
                .AddSingleton<IPhoneNumberParser, PhoneNumberParser>()
                .AddTransient<IUsersBanService, UsersBanService>();

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