using System;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Infrastructure.AccidentReporting;
using MotoHealth.Infrastructure.ChatStorage;

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

            services.Configure(configurator.ConfigureChatStorage);

            services
                .AddSingleton<ICloudTablesProvider, CloudTablesProvider>()
                .AddSingleton<IAzureTablesInitializer, AzureTablesInitializer>()
                .AddSingleton<IDefaultChatStateFactory, DefaultChatStateFactory>()
                .AddSingleton<IChatStateInMemoryCache, ChatStateInMemoryCache>()
                .AddSingleton<IChatStatesStore, AzureTableChatStatesStore>()
                .AddSingleton<IAccidentReportingService, NoOpAccidentReportingService>();

            services.AddHostedService<AzureTablesInitializerHostedService>();

            return services;
        }
    }
}