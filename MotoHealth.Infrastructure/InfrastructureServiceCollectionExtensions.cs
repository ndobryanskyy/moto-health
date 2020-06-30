﻿using System;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Infrastructure.AccidentReporting;
using MotoHealth.Infrastructure.AzureEventGrid;
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
                .AddSingleton<ICloudTablesProvider, CloudTablesProvider>()
                .AddSingleton<IAzureTablesInitializer, AzureTablesInitializer>()
                .AddSingleton<IDefaultChatStateFactory, DefaultChatStateFactory>()
                .AddSingleton<IChatStateInMemoryCache, ChatStateInMemoryCache>()
                .AddSingleton<IChatStatesStore, AzureTableChatStatesStore>()
                .AddSingleton<IChatSubscriptionsService, AzureTablesChatSubscriptionsService>()
                .AddScoped<IAccidentReportingService, AzureEventGridAccidentReportingService>();
                
            services
                .AddHttpClient<IAppEventsTopicClient, AppEventsTopicClient>()
                .SetHandlerLifetime(TimeSpan.FromHours(1));

            services.AddHostedService<AzureTablesInitializerHostedService>();

            return services;
        }
    }
}