using Telegram.Bot.Types;

namespace MotoHealth.Bot.Telegram.Updates
{
    internal abstract class BotUpdateBase : IBotUpdate
    {
        protected BotUpdateBase(Update update)
        {
            OriginalUpdate = update;
        }

        public Update OriginalUpdate { get; }
        
        public abstract Chat Chat { get; }
    }
}