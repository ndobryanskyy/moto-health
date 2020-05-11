using System;

namespace MotoHealth.Telegram
{
    public sealed class TelegramOptions
    {
        public string BotId { get; set; } = string.Empty;

        public string BotSecret { get; set; } = string.Empty;

        public int RequestTimeoutInSeconds { get; set; } = 30;

        public string BotToken => $"{BotId}:{BotSecret}";

        public TimeSpan RequestTimeout => TimeSpan.FromSeconds(RequestTimeoutInSeconds);
    }
}