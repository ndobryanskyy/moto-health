﻿namespace MotoHealth.Bot.Telegram
{
    public sealed class TelegramOptions
    {
        public string BotId { get; set; } = string.Empty;

        public string BotSecret { get; set; } = string.Empty;

        public string BotToken => $"{BotId}:{BotSecret}";
    }
}