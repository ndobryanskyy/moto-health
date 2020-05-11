using System.Net.Http;
using Telegram.Bot;

namespace MotoHealth.Telegram
{
    internal sealed class TelegramBotClientFactory : ITelegramBotClientFactory
    {
        public ITelegramBotClient CreateClient(TelegramOptions options)
        {
            var httpClient = new HttpClient
            {
                Timeout = options.RequestTimeout
            };

            return new TelegramBotClient(options.BotToken, httpClient);
        }
    }
}