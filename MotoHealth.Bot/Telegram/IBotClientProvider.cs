using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace MotoHealth.Bot.Telegram
{
    public interface IBotClientProvider
    {
        ITelegramBotClient Client { get; }
    }

    internal sealed class BotClientProvider : IBotClientProvider
    {
        public BotClientProvider(IOptions<TelegramOptions> options)
        {
            var telegramOptions = options.Value;

            Client = new TelegramBotClient(telegramOptions.BotToken);
        }

        public ITelegramBotClient Client { get; }
    }
}