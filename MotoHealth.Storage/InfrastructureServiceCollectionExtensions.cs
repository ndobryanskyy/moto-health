using System;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Infrastructure.ChatStorage;
using MotoHealth.Infrastructure.ServiceBus;
using MotoHealth.Infrastructure.UpdatesQueue;

namespace MotoHealth.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            InfrastructureOptionsConfigurator configurator)
        {
            if (configurator.ConfigureUpdatesQueue == null)
            {
                throw new InvalidOperationException($"{nameof(InfrastructureOptionsConfigurator.ConfigureUpdatesQueue)} must be set");
            }

            if (configurator.ConfigureChatStorage == null)
            {
                throw new InvalidOperationException($"{nameof(InfrastructureOptionsConfigurator.ConfigureChatStorage)} must be set");
            }

            services.Configure(configurator.ConfigureUpdatesQueue);
            services.Configure(configurator.ConfigureChatStorage);

            services
                .AddSingleton<ICloudTablesProvider, CloudTablesProvider>()
                .AddSingleton<IDefaultChatStateFactory, DefaultChatStateFactory>()
                .AddSingleton<IChatStateInMemoryCache, ChatStateInMemoryCache>()
                .AddSingleton<IChatStatesStore, AzureTableChatStatesStore>()
                .AddSingleton<IServiceBusClientsFactory, ServiceBusClientsFactory>()
                .AddSingleton<IBotUpdatesSerializer, BotUpdatesSerializer>()
                .AddSingleton<IBotUpdatesQueue, ServiceBusUpdatesQueue>();

            services.AddHostedService<TableStorageMigrationsHostedService>();
            services.AddHostedService<UpdatesQueueHandlerBackgroundService>();

            return services;
        }
    }
}