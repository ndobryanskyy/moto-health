using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MotoHealth.Telegram.Messages
{
    public sealed class CompositeMessageBuilder : IMessage
    {
        private readonly List<IMessage> _innerMessages = new List<IMessage>();

        public CompositeMessageBuilder AddMessage(IMessage message)
        {
            _innerMessages.Add(message);

            return this;
        }

        public async Task SendAsync(
            ChatId chatId,
            ITelegramBotClient client,
            CancellationToken cancellationToken)
        {
            foreach (var message in _innerMessages)
            {
                await message.SendAsync(chatId, client, cancellationToken);
            }
        }
    }
}