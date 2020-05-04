using Telegram.Bot;

namespace MotoHealth.Telegram
{
    internal sealed class TelegramBotClientFactory : ITelegramBotClientFactory
    {
        public ITelegramBotClient CreateClient(TelegramOptions options) 
            => new TelegramBotClient(options.BotToken);
    }
}