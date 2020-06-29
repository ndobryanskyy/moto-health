using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MotoHealth.Bot.AppInsights;
using MotoHealth.Bot.Authorization;
using MotoHealth.Bot.Middleware;
using MotoHealth.Bot.Telegram;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Bot
{
    internal static class AppServiceCollectionExtensions
    {
        public static IServiceCollection AddApp(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureAppInsights();

            services.AddTransient(typeof(ChatUpdateHandlerMiddleware<>));

            services
                .AddTransient<ReliableUpdateHandlingContextMiddleware>()
                .AddSingleton<BotTokenVerificationMiddleware>()
                .AddSingleton<BotUpdateInitializerMiddleware>()
                .AddTransient<ChatUpdatesFilterMiddleware>()
                .AddSingleton<ChatLockingMiddleware>()
                .AddTransient<NewChatsHandlerMiddleware>()
                .AddTransient<TerminatingChatHandlerMiddleware>();

            services
                .AddSingleton<IAuthorizationSecretsService, AuthorizationSecretsService>()
                .AddScoped<IBotTelemetryService, AppInsightBotTelemetryService>();
                
            services
                .AddHostedService<BotInitializerStartupJob>();

            return services;

            void ConfigureAppInsights()
            {
                services.AddApplicationInsightsTelemetry(configuration);

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