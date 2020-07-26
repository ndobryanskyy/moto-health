using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Core.Bot.ChatUpdateHandlers;
using MotoHealth.Core.Bot.Commands;
using MotoHealth.Core.Bot.Commands.AppCommands;
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
                .AddSingleton<IPublicCommandsProvider, PublicCommandsProvider>()
                .AddSingleton<IBotCommand, StartBotCommand>()
                .AddSingleton<IBotCommand, MotoHealthInfoBotCommand>()
                .AddTransient<IBotCommand, ReportAccidentBotCommand>()
                .AddTransient<IBotCommand, SubscribeChatToAccidentAlertingBotCommand>()
                .AddTransient<IBotCommand, UnsubscribeChatFromAccidentAlertingBotCommand>()
                .AddTransient<IBotCommand, BanUserBotCommand>()
                .AddTransient<IBotCommand, UnbanUserBotCommand>();

            services
                .AddSingleton<IBotUpdatesMapper, BotUpdatesMapper>()
                .AddScoped<IChatFactory, ChatFactory>()
                .AddTransient<IChatStatesRepository, ChatStatesRepository>()
                .AddSingleton<IChatsDoorman, ChatsDoorman>()
                .AddSingleton<IPhoneNumberParser, PhoneNumberParser>()
                .AddTransient<IUsersBanService, UsersBanService>();

            services
                .AddTransient<AccidentReportingDialogChatUpdateHandler>()
                .AddTransient<IAccidentReportingDialogHandler, AccidentReportingDialogHandler>()
                .AddSingleton<IAccidentReportingDialogMessages, AccidentReportingDialogMessages>();

            services.AddTransient<MainCommandsChatUpdateHandler>();

            return services;
        }
    }
}