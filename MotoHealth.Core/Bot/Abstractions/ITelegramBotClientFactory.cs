using Telegram.Bot;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface ITelegramBotClientFactory
    {
        ITelegramBotClient CreateClient();
    }
}