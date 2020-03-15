using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public abstract class BotUpdateBase : IBotUpdate
    {
        protected BotUpdateBase(int updateId)
        {
            UpdateId = updateId;
        }

        public int UpdateId { get; }

        public abstract IChatContext Chat { get; }
    }
}