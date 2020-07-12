using Telegram.Bot.Types;

namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface INotMappedBotUpdate : IBotUpdate
    {
        Update OriginalUpdate { get; }
    }
}