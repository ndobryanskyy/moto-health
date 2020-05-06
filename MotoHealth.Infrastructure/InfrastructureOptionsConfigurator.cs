using System;
using MotoHealth.Infrastructure.AzureEventGrid;
using MotoHealth.Infrastructure.ChatStorage;

namespace MotoHealth.Infrastructure
{
    public sealed class InfrastructureOptionsConfigurator
    {
        public Action<ChatStorageOptions>? ConfigureChatStorage { get; set; }

        public Action<AzureEventGridOptions>? ConfigureEventGrid { get; set; }
    }
}