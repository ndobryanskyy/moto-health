using Telegram.Bot;

namespace MotoHealth.Bot.Telegram
{
    internal interface ITelegramBotClientFactory
    {
        ITelegramBotClient CreateClient(string botToken);
    }

    internal sealed class TelegramBotClientFactory : ITelegramBotClientFactory
    {
        public ITelegramBotClient CreateClient(string botToken) => new TelegramBotClient(botToken);
    }
}