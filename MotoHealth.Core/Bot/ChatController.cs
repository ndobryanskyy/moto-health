using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatController : IChatController
    {
        public ChatController(
            long chatId,
            ITelegramBotClient botClient)
        {
            ChatId = chatId;

            TelegramClient = botClient;
        }

        public long ChatId { get; }

        public ITelegramBotClient TelegramClient { get; }

        public async Task HandleUpdateAsync(IBotUpdateContext context, CancellationToken cancellationToken)
        {
            if (context.Update is ITextMessageBotUpdate textMessage)
            {
                await context.SendTextMessageAsync(textMessage.Text, cancellationToken);
            }
        }
    }
}