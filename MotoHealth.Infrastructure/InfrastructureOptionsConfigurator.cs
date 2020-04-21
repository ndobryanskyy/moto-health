using System;
using MotoHealth.Infrastructure.AccidentReporting;
using MotoHealth.Infrastructure.ChatStorage;

namespace MotoHealth.Infrastructure
{
    public sealed class InfrastructureOptionsConfigurator
    {
        public Action<ChatStorageOptions>? ConfigureChatStorage { get; set; }

        public Action<AccidentsQueueOptions>? ConfigureAccidentsQueue { get; set; }
    }
}