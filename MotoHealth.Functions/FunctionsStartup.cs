using System;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Functions.AccidentAlerting;
using MotoHealth.Functions.AdminBot;
using MotoHealth.Functions.Authorization;
using MotoHealth.Functions.ChatSubscriptions;
using MotoHealth.Telegram;
using AzureFunctionsStartup = Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup;

[assembly: FunctionsStartup(typeof(MotoHealth.Functions.FunctionsStartup))]

namespace MotoHealth.Functions
{
    public sealed class FunctionsStartup : AzureFunctionsStartup
    {
        private const string TelegramConfigurationSection = "Telegram";
        private const string SubscriptionSecretConfigurationKey = "SubscriptionSecret";
        private const string StorageAccountConfigurationKey = "StorageAccount";

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
                .AddSingleton(CreateSharedTableClient)
                .AddSingleton<ICloudTablesProvider, CloudTablesProvider>()
                .AddSingleton<IBotTokenValidator, BotTokenValidator>()
                .AddSingleton<IAccidentAlertingSubscriptionsManager, AccidentAlertingSubscriptionsManager>()
                .AddSingleton<IAccidentRecordingService, AccidentRecordingService>()
                .AddSingleton<IChatSubscriptionsManager, ChatSubscriptionsManager>()
                .AddSingleton<IAdminBot, AdminBot.AdminBot>()
                .AddSingleton<IAuthorizationService, AuthorizationService>();
        }

        private static CloudTableClient CreateSharedTableClient(IServiceProvider container)
        {
            var configuration = container.GetRequiredService<IConfiguration>();

            var storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString(StorageAccountConfigurationKey));
            return storageAccount.CreateCloudTableClient();
        }
    }
}