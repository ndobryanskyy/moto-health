using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MotoHealth.Telegram.Messages
{
    public interface IMessage
    {
        Task SendAsync(
            ChatId chatId, 
            ITelegramBotClient client, 
            CancellationToken cancellationToken = default);
    }
}