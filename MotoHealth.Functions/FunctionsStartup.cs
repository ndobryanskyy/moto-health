using System;
using AutoMapper;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Functions.AccidentAlerting;
using MotoHealth.Telegram;
using AzureFunctionsStartup = Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup;

[assembly: FunctionsStartup(typeof(MotoHealth.Functions.FunctionsStartup))]

namespace MotoHealth.Functions
{
    public sealed class FunctionsStartup : AzureFunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddAutoMapper(typeof(FunctionsStartup));

            builder.Services.AddTelegram();

            builder.Services
                .AddOptions<TelegramClientOptions>()
                .Configure<IConfiguration>((telegramOptions, configuration) =>
                {
                    configuration
                        .GetSection(Constants.Telegram.ConfigurationSectionName)
                        .Bind(telegramOptions);
                });

            builder.Services
                .AddSingleton(CreateSharedTableClient)
                .AddSingleton<ICloudTablesProvider, CloudTablesProvider>()
                .AddSingleton<IGoogleMapsService, GoogleMapsService>()
                .AddSingleton<IAccidentRecordingService, AccidentRecordingService>();
        }

        private static CloudTableClient CreateSharedTableClient(IServiceProvider container)
        {
            var configuration = container.GetRequiredService<IConfiguration>();

            var storageAccount = CloudStorageAccount.Parse(
                configuration.GetSection(Constants.AzureStorage.StorageAccountConnectionStringName).Value);

            return storageAccount.CreateCloudTableClient();
        }
    }
}