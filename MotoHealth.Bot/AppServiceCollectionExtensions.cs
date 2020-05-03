using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MotoHealth.Bot.AppInsights;
using MotoHealth.Bot.Telegram;
using MotoHealth.Core.Bot;

namespace MotoHealth.Bot
{
    internal static class AppServiceCollectionExtensions
    {
        public static IServiceCollection AddApp(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureAppInsights();

            services
                .AddSingleton<IBotUpdateAccessor, BotUpdateAccessor>()
                .AddScoped<IBotTelemetryService, AppInsightBotTelemetryService>();
                
            services
                .AddHostedService<BotInitializerStartupJob>();

            return services;

            void ConfigureAppInsights()
            {
                services.AddApplicationInsightsTelemetry(configuration);

                services.RemoveAll<ITelemetryInitializer>();

                services
                    .AddSingleton<ITelemetryInitializer, DomainNameRoleInstanceTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, AzureAppServiceRoleNameFromHostNameHeaderInitializer>()
                    .AddSingleton<ITelemetryInitializer, ComponentVersionTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, AspNetCoreEnvironmentTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, HttpDependenciesParsingTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, SyntheticTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, ClientIpHeaderTelemetryInitializer>();

                services
                    .AddSingleton<ITelemetryInitializer, TelegramRequestTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, BotUpdateContextTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, TelegramDependencyTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, AzureTableDependencyTelemetryInitializer>();
            }
        }
    }
}