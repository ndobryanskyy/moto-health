using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatUpdateContext : IChatUpdateContext
    {
        private readonly long _chatId;

        private readonly ITelegramClient _client;

        public ChatUpdateContext(IChatUpdate update, ITelegramClient client)
        {
            _client = client;

            Update = update;

            _chatId = Update.Chat.Id;
        }

        public IChatUpdate Update { get; }

        public async Task SendMessageAsync(IMessage message, CancellationToken cancellationToken)
        {
            await message.SendAsync(_chatId, _client, cancellationToken);
        }
    }
}