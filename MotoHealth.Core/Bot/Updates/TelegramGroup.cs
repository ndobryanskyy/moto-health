using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public sealed class TelegramGroup : ITelegramGroup
    {
        public string? Title { get; set; }
    }
}