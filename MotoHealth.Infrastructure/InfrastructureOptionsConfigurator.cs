using System;
using MotoHealth.Infrastructure.ChatStorage;

namespace MotoHealth.Infrastructure
{
    public sealed class InfrastructureOptionsConfigurator
    {
        public Action<ChatStorageOptions>? ConfigureChatStorage { get; set; }
    }
}