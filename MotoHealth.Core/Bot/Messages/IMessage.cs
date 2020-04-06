using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MotoHealth.Core.Bot.Messages
{
    public interface IMessage
    {
        Task SendAsync(
            ChatId chatId, 
            ITelegramBotClient client, 
            CancellationToken cancellationToken);
    }
}