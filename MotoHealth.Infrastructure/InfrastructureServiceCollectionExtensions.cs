using System;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Infrastructure.AccidentReporting;
using MotoHealth.Infrastructure.ChatStorage;
using MotoHealth.Infrastructure.ServiceBus;

namespace MotoHealth.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            InfrastructureOptionsConfigurator configurator)
        {
            if (configurator.ConfigureChatStorage == null)
            {
                throw new InvalidOperationException($"{nameof(InfrastructureOptionsConfigurator.ConfigureChatStorage)} must be set");
            }

            if (configurator.ConfigureAccidentsQueue == null)
            {
                throw new InvalidOperationException($"{nameof(InfrastructureOptionsConfigurator.ConfigureAccidentsQueue)} must be set");
            }

            services.Configure(configurator.ConfigureChatStorage);
            services.Configure(configurator.ConfigureAccidentsQueue);

            services
                .AddSingleton<ICloudTablesProvider, CloudTablesProvider>()
                .AddSingleton<IAzureTablesInitializer, AzureTablesInitializer>()
                .AddSingleton<IDefaultChatStateFactory, DefaultChatStateFactory>()
                .AddSingleton<IChatStateInMemoryCache, ChatStateInMemoryCache>()
                .AddSingleton<IChatStatesStore, AzureTableChatStatesStore>()
                .AddSingleton<IServiceBusClientsFactory, ServiceBusClientsFactory>()
                .AddSingleton<IAccidentsQueue, ServiceBusAccidentsQueue>();

            services.AddHostedService<AzureTablesInitializerHostedService>();

            return services;
        }
    }
}