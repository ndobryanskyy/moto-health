using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace MotoHealth.Bot.Telegram
{
    internal interface ITelegramBotClientFactory
    {
        ITelegramBotClient CreateClient();
    }

    internal sealed class TelegramBotClientFactory : ITelegramBotClientFactory
    {
        private readonly string _botToken;

        public TelegramBotClientFactory(IOptions<TelegramOptions> options)
        {
            _botToken = options.Value.BotToken;
        }

        public ITelegramBotClient CreateClient() => new TelegramBotClient(_botToken);
    }
}