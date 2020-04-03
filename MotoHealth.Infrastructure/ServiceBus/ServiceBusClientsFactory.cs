using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace MotoHealth.Infrastructure.ServiceBus
{
    internal interface IServiceBusClientsFactory
    {
        IQueueClient CreateSessionHandlingClient(ServiceBusConnectionStringBuilder connectionStringBuilder);

        IMessageSender CreateMessageSenderClient(ServiceBusConnectionStringBuilder connectionStringBuilder);
    }

    internal sealed class ServiceBusClientsFactory : IServiceBusClientsFactory
    {
        public IQueueClient CreateSessionHandlingClient(ServiceBusConnectionStringBuilder connectionStringBuilder) 
            => new QueueClient(connectionStringBuilder);

        public IMessageSender CreateMessageSenderClient(ServiceBusConnectionStringBuilder connectionStringBuilder) 
            => new MessageSender(connectionStringBuilder);
    }
}