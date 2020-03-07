using Telegram.Bot.Types;

namespace MotoHealth.Bot.Telegram.Updates
{
    public interface IBotUpdate
    {
        Update OriginalUpdate { get; }
        
        Chat Chat { get; }
    }
}