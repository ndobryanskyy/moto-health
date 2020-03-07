using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Configuration;

namespace MotoHealth.Bot.ServiceBus
{
    public interface IMessagesQueueSenderClientProvider
    {
        IMessageSender Client { get; }
    }

    public interface IMessagesQueueReceiverClientProvider
    {
        IQueueClient Client { get; }
    }

    internal sealed class QueueClientsProvider : IMessagesQueueSenderClientProvider, IMessagesQueueReceiverClientProvider
    {
        private readonly MessageSender _telegramMessagesSenderClient;
        private readonly QueueClient _telegramMessagesReceiverClient;

        public QueueClientsProvider(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(Constants.UpdatesQueue.ConnectionStringName);
            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(connectionString);

            _telegramMessagesSenderClient = new MessageSender(connectionStringBuilder);
            _telegramMessagesReceiverClient = new QueueClient(connectionStringBuilder);
        }

        IMessageSender IMessagesQueueSenderClientProvider.Client => _telegramMessagesSenderClient;

        IQueueClient IMessagesQueueReceiverClientProvider.Client => _telegramMessagesReceiverClient;
    }
}