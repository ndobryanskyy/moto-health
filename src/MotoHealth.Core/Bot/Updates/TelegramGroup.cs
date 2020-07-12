using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    internal sealed class TelegramGroup : ITelegramGroup
    {
        public string? Title { get; set; }
    }
}