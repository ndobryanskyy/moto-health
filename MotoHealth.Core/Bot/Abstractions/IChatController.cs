using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatController
    {
        long ChatId { get; }

        ITelegramBotClient TelegramClient { get; }

        Task HandleUpdateAsync(IBotUpdateContext context, CancellationToken cancellationToken);
    }
}