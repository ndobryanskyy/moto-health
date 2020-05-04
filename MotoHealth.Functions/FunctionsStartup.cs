using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Functions.AdminBot;
using MotoHealth.Telegram;
using AzureFunctionsStartup = Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup;

[assembly: FunctionsStartup(typeof(MotoHealth.Functions.FunctionsStartup))]

namespace MotoHealth.Functions
{
    public sealed class FunctionsStartup : AzureFunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<TelegramOptions>()
                .Configure<IConfiguration>((telegramOptions, configuration) =>
                {
                    configuration.GetSection("Telegram").Bind(telegramOptions);
                });

            builder.Services.AddTelegram();

            builder.Services.AddSingleton<IBotTokenValidator, BotTokenValidator>();
        }
    }
}