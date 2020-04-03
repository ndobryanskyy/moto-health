using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Infrastructure.ServiceBus;

namespace MotoHealth.Infrastructure.UpdatesQueue
{
    internal sealed class ServiceBusUpdatesQueue : IBotUpdatesQueue
    {
        private readonly ILogger<IBotUpdatesQueue> _logger;
        private readonly IBotUpdatesSerializer _serializer;
        private readonly IMessageSender _senderClient;

        public ServiceBusUpdatesQueue(
            ILogger<IBotUpdatesQueue> logger,
            IOptions<UpdatesQueueOptions> updatesQueueOptions,
            IBotUpdatesSerializer serializer,
            IServiceBusClientsFactory clientsFactory)
        {
            _logger = logger;
            _serializer = serializer;

            var connectionString = updatesQueueOptions.Value.ConnectionString;
            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(connectionString);

            _senderClient = clientsFactory.CreateMessageSenderClient(connectionStringBuilder);
        }

        public async Task EnqueueUpdateAsync(IBotUpdate botUpdate, CancellationToken cancellationToken)
        {
            var serialized = _serializer.SerializeToMessageBody(botUpdate);

            _logger.LogDebug($"Successfully serialized update {botUpdate.UpdateId} to message body");

            var message = new Message(serialized)
            {
                SessionId = botUpdate.Chat.Id.ToString()
            };

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await _senderClient.SendAsync(message);

            _logger.LogDebug($"Successfully added update {botUpdate.UpdateId} to updates queue");
        }
    }
}