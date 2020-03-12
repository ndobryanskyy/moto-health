using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf;
using Microsoft.Azure.ServiceBus.Core;
using MotoHealth.Bot.ServiceBus;
using MotoHealth.Bot.Telegram.Updates;
using MotoHealth.BotUpdates;
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
        private readonly IMapper _mapper;
        private readonly ISenderClient _senderClient;

        public TelegramUpdatesQueue(
            IMessagesQueueSenderClientProvider clientProvider, 
            IMapper mapper)
        {
            _mapper = mapper;
            _senderClient = clientProvider.Client;
        }

        public async Task AddUpdateAsync(IBotUpdate botUpdate, CancellationToken cancellationToken)
        {
            var mapped = _mapper.Map<BotUpdateDto>(botUpdate);
            var serialized = mapped.ToByteArray();

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