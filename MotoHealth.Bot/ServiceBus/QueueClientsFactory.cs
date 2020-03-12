using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace MotoHealth.Bot.ServiceBus
{
    public interface IQueueClientsFactory
    {
        IQueueClient CreateSessionHandlingClient(ServiceBusConnectionStringBuilder connectionStringBuilder);

        IMessageSender CreateMessageSenderClient(ServiceBusConnectionStringBuilder connectionStringBuilder);
    }

    internal sealed class QueueClientsFactory : IQueueClientsFactory
    {
        public IQueueClient CreateSessionHandlingClient(ServiceBusConnectionStringBuilder connectionStringBuilder) 
            => new QueueClient(connectionStringBuilder);

        public IMessageSender CreateMessageSenderClient(ServiceBusConnectionStringBuilder connectionStringBuilder) 
            => new MessageSender(connectionStringBuilder);
    }
}