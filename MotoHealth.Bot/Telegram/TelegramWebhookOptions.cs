using Telegram.Bot.Types.Enums;

namespace MotoHealth.Bot.Telegram
{
    internal sealed class TelegramWebhookOptions
    {
        public string BaseUrl { get; set; } = string.Empty;

        public int MaxConnections { get; set; } = 10;

        public UpdateType[] AllowedUpdates { get; } = { UpdateType.Message };
    }
}