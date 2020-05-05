using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Functions.AdminBot;
using MotoHealth.Functions.AdminBot.Authorization;
using MotoHealth.Functions.AdminBot.ChatSubscriptions;
using MotoHealth.Telegram;
using AzureFunctionsStartup = Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup;

[assembly: FunctionsStartup(typeof(MotoHealth.Functions.FunctionsStartup))]

namespace MotoHealth.Functions
{
    public sealed class FunctionsStartup : AzureFunctionsStartup
    {
        private const string TelegramConfigurationSection = "Telegram";
        private const string SubscriptionSecretConfigurationKey = "SubscriptionSecret";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<TelegramOptions>()
                .Configure<IConfiguration>((telegramOptions, configuration) =>
                {
                    configuration.GetSection(TelegramConfigurationSection).Bind(telegramOptions);
                });

            builder.Services.AddOptions<AuthorizationOptions>()
                .Configure<IConfiguration>((authorizationOptions, configuration) =>
                {
                    authorizationOptions.SubscriptionSecret = configuration.GetValue<string>(SubscriptionSecretConfigurationKey);
                });

            builder.Services.AddTelegram();

            builder.Services
                .AddSingleton<IBotTokenValidator, BotTokenValidator>()
                .AddSingleton<IAccidentAlertingService, AccidentAlertingService>()
                .AddSingleton<IChatSubscriptionsManager, ChatSubscriptionsManager>()
                .AddSingleton<IAdminBot, AdminBot.AdminBot>()
                .AddSingleton<IAuthorizationService, AuthorizationService>();
        }
    }
}