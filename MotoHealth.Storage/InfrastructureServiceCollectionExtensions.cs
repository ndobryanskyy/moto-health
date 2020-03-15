using System;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Infrastructure.ChatControllersStorage;
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

            services.Configure(configurator.ConfigureUpdatesQueue);

            services
                .AddSingleton<IChatControllersInMemoryCache, ChatControllersInMemoryCache>()
                .AddSingleton<IChatControllersStore, NoOpChatControllersStore>()
                .AddSingleton<IServiceBusClientsFactory, ServiceBusClientsFactory>()
                .AddSingleton<IBotUpdatesSerializer, BotUpdatesSerializer>()
                .AddSingleton<IBotUpdatesQueue, ServiceBusUpdatesQueue>();

            services.AddHostedService<UpdatesQueueHandlerBackgroundService>();

            return services;
        }
    }
}