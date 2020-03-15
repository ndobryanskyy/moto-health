using System;
using MotoHealth.Infrastructure.UpdatesQueue;

namespace MotoHealth.Infrastructure
{
    public sealed class InfrastructureOptionsConfigurator
    {
        public Action<UpdatesQueueOptions>? ConfigureUpdatesQueue { get; set; }
    }
}