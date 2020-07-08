using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Bot.AppInsights
{
    public static class AppInsightsApplicationExtensions
    {
        private const string AppInsightsLinuxTempPath = "/tmp/app-insights";

        private static string ApplicationVersion
            => typeof(Startup).Assembly.GetName().Version?.ToString() ?? throw new InvalidOperationException();

        public static IServiceCollection AddAppInsights(this IServiceCollection services, IConfiguration configuration)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Directory.CreateDirectory(AppInsightsLinuxTempPath);
                
                services.AddSingleton<ITelemetryChannel>(new ServerTelemetryChannel
                {
                    StorageFolder = AppInsightsLinuxTempPath
                });
            }

            services.AddOptions<ApplicationInsightsServiceOptions>()
                .Bind(configuration.GetSection(Constants.ApplicationInsights.ConfigurationSectionName))
                .PostConfigure(options => options.ApplicationVersion = ApplicationVersion);

            services.AddApplicationInsightsTelemetry(options =>
            {
                // TODO investigate ETW memory leak and re-enable
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

            services
                .AddSingleton<IBotTelemetryService, AppInsightBotTelemetryService>()
                .AddSingleton<ITelegramTelemetrySanitizer, TelegramTelemetrySanitizer>();

            return services;
        }
    }
}