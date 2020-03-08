using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Core;
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
        private readonly ISenderClient _senderClient;

        public TelegramUpdatesQueue(IMessagesQueueSenderClientProvider clientProvider)
        {
            _senderClient = clientProvider.Client;
        }

        public async Task AddUpdateAsync(IBotUpdate botUpdate, CancellationToken cancellationToken)
        {
            var convertedBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(botUpdate));

            var message = new Message(convertedBody)
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