using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Infrastructure.AccidentReporting;
using MotoHealth.Infrastructure.AzureStorageQueue;
using MotoHealth.Infrastructure.AzureTables;
using MotoHealth.Infrastructure.ChatsState;
using MotoHealth.Infrastructure.ChatSubscriptions;

namespace MotoHealth.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            Action<InfrastructureOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services
                .AddHealthChecks()
                .AddAzureTables()
                .AddAzureStorageQueues();

            services
                .AddSingleton<ICloudTablesProvider, CloudTablesProvider>()
                .AddSingleton<IAzureStorageInitializer, AzureStorageInitializer>()
                .AddSingleton<IChatStateInMemoryCache, ChatStateInMemoryCache>()
                .AddSingleton<IChatStatesStore, AzureTableChatStatesStore>()
                .AddSingleton<IChatSubscriptionsService, AzureTablesChatSubscriptionsService>()
                .AddScoped<IAccidentReportingService, AzureStorageQueueAccidentReportingService>();

            services
                .AddHttpClient<IAppEventsQueuesClient, AppEventsAzureStorageQueuesClient>()
                .ConfigureHttpClient((container, client) =>
                {
                    var options = container.GetRequiredService<IOptions<InfrastructureOptions>>().Value;

                    client.Timeout = options.AzureStorage.QueuesRequestTimeout;
                });

            services.AddTransient(container => (IAppQueuesStatusProvider)container.GetRequiredService<IAppEventsQueuesClient>());

            services.AddHostedService<AzureStorageInitializerHostedService>();

            return services;
        }
    }
}