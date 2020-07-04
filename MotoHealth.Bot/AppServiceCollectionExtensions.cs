using System;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MotoHealth.Bot.AppInsights;
using MotoHealth.Bot.Authorization;
using MotoHealth.Bot.Telegram;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Bot
{
    internal static class AppServiceCollectionExtensions
    {
        private static string ApplicationVersion
            => typeof(Startup).Assembly.GetName().Version?.ToString() ?? throw new InvalidOperationException();

        public static IServiceCollection AddApp(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTelegramBot();

            ConfigureAppInsights();

            services
                .AddSingleton<IAuthorizationSecretsService, AuthorizationSecretsService>()
                .AddScoped<IBotTelemetryService, AppInsightBotTelemetryService>();
                
            services
                .AddHostedService<BotInitializerStartupJob>();

            return services;

            void ConfigureAppInsights()
            {
                services.AddOptions<ApplicationInsightsServiceOptions>()
                    .Bind(configuration.GetSection(Constants.ApplicationInsights.ConfigurationSectionName))
                    .PostConfigure(options => options.ApplicationVersion = ApplicationVersion);

                services.AddApplicationInsightsTelemetry(options =>
                {
                    // TODO investigate memory leak and re-enable
                    options.EnablePerformanceCounterCollectionModule = false;
                    options.EnableEventCounterCollectionModule = false;
                });

                services.RemoveAll<ITelemetryInitializer>();

                services
                    .AddApplicationInsightsTelemetryProcessor<AlwaysOnPingFilteringTelemetryProcessor>()
                    .AddSingleton<ITelemetryInitializer, DomainNameRoleInstanceTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, AzureAppServiceRoleNameFromHostNameHeaderInitializer>()
                    .AddSingleton<ITelemetryInitializer, ComponentVersionTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, AspNetCoreEnvironmentTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, HttpDependenciesParsingTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, SyntheticTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, ClientIpHeaderTelemetryInitializer>();

                services
                    .AddSingleton<ITelemetryInitializer, BotUpdateContextTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, TelegramDependencyTelemetryInitializer>()
                    .AddSingleton<ITelemetryInitializer, AzureTableDependencyTelemetryInitializer>();
            }
        }
    }
}