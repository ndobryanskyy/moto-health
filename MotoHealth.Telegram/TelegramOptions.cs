using System;

namespace MotoHealth.Telegram
{
    public sealed class TelegramOptions
    {
        private const string TelegramBotApiBaseUrl = "https://api.telegram.org/bot";

        public string BotId { get; set; } = string.Empty;

        public string BotSecret { get; set; } = string.Empty;

        public int RequestTimeoutInSeconds { get; set; } = 15;

        public Uri BaseAddress => new Uri($"{TelegramBotApiBaseUrl}{BotId}:{BotSecret}/");

        public TimeSpan RequestTimeout => TimeSpan.FromSeconds(RequestTimeoutInSeconds);
    }
}