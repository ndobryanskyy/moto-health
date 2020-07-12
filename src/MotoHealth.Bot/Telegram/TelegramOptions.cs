using MotoHealth.Telegram;

namespace MotoHealth.Bot.Telegram
{
    internal sealed class TelegramOptions
    {
        public TelegramClientOptions Client { get; } = new TelegramClientOptions();

        public TelegramWebhookOptions Webhook { get; } = new TelegramWebhookOptions();
    }
}