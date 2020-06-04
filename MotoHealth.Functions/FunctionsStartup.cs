using System;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Functions.AccidentAlerting;
using MotoHealth.Functions.ChatSubscriptions;
using MotoHealth.Telegram;
using AzureFunctionsStartup = Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup;

[assembly: FunctionsStartup(typeof(MotoHealth.Functions.FunctionsStartup))]

namespace MotoHealth.Functions
{
    public sealed class FunctionsStartup : AzureFunctionsStartup
    {
        private const string TelegramConfigurationSection = "Telegram";
        private const string StorageAccountConfigurationKey = "StorageAccount";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<TelegramOptions>()
                .Configure<IConfiguration>((telegramOptions, configuration) =>
                {
                    configuration.GetSection(TelegramConfigurationSection).Bind(telegramOptions);
                });

            builder.Services.AddTelegram();

            builder.Services
                .AddSingleton(CreateSharedTableClient)
                .AddSingleton<IEventGridEventDataParser, EventGridEventDataParser>()
                .AddSingleton<ICloudTablesProvider, CloudTablesProvider>()
                .AddSingleton<IAccidentAlertingSubscriptionsService, AccidentAlertingSubscriptionsService>()
                .AddSingleton<IAccidentRecordingService, AccidentRecordingService>()
                .AddSingleton<IChatSubscriptionsEventsStore, ChatSubscriptionsEventsStore>();
        }

        private static CloudTableClient CreateSharedTableClient(IServiceProvider container)
        {
            var configuration = container.GetRequiredService<IConfiguration>();

            var storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString(StorageAccountConfigurationKey));
            return storageAccount.CreateCloudTableClient();
        }
    }
}