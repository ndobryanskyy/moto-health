using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Options;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Infrastructure.ServiceBus;

namespace MotoHealth.Infrastructure.UpdatesQueue
{
    internal sealed class ServiceBusUpdatesQueue : IBotUpdatesQueue
    {
        private readonly IBotUpdatesSerializer _serializer;
        private readonly IMessageSender _senderClient;

        public ServiceBusUpdatesQueue(
            IOptions<UpdatesQueueOptions> updatesQueueOptions,
            IBotUpdatesSerializer serializer,
            IServiceBusClientsFactory clientsFactory)
        {
            _serializer = serializer;

            var connectionString = updatesQueueOptions.Value.ConnectionString;
            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(connectionString);

            _senderClient = clientsFactory.CreateMessageSenderClient(connectionStringBuilder);
        }

        public async Task EnqueueUpdateAsync(
            IBotUpdate botUpdate, 
            CancellationToken cancellationToken)
        {
            var serialized = _serializer.SerializeToMessageBody(botUpdate);

            var message = new Message(serialized)
            {
                SessionId = botUpdate.Chat.Id.ToString()
            };

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await _senderClient.SendAsync(message);
        }
    }
}