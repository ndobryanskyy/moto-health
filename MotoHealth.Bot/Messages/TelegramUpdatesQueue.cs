using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Configuration;
using MotoHealth.Bot.ServiceBus;
using MotoHealth.Bot.Telegram.Updates;
using Message = Microsoft.Azure.ServiceBus.Message;

namespace MotoHealth.Bot.Messages
{
    public interface ITelegramUpdatesQueue
    {
        Task AddUpdateAsync(
            IBotUpdate botUpdate,
            CancellationToken cancellationToken);
    }

    internal sealed class TelegramUpdatesQueue : ITelegramUpdatesQueue
    {
        private readonly IBotUpdateSerializer _serializer;
        private readonly ISenderClient _senderClient;

        public TelegramUpdatesQueue(
            IConfiguration configuration,
            IQueueClientsFactory clientsFactory,
            IBotUpdateSerializer serializer)
        {
            _serializer = serializer;

            var connectionString = configuration.GetConnectionString(Constants.UpdatesQueue.ConnectionStringName);
            var builder = new ServiceBusConnectionStringBuilder(connectionString);

            _senderClient = clientsFactory.CreateMessageSenderClient(builder);
        }

        public async Task AddUpdateAsync(IBotUpdate botUpdate, CancellationToken cancellationToken)
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