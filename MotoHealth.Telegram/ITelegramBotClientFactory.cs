using Telegram.Bot;

namespace MotoHealth.Telegram
{
    public interface ITelegramBotClientFactory
    {
        ITelegramBotClient CreateClient(TelegramOptions options);
    }
}