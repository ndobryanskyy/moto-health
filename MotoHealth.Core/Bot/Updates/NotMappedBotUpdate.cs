using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types;

namespace MotoHealth.Core.Bot.Updates
{
    internal sealed class NotMappedBotUpdate : BotUpdateBase, INotMappedBotUpdate
    {
        public Update OriginalUpdate { get; set; } = default!;
    }
}