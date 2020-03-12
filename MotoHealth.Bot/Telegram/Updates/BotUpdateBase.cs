namespace MotoHealth.Bot.Telegram.Updates
{
    internal abstract class BotUpdateBase : IBotUpdate
    {
        protected BotUpdateBase(int updateId)
        {
            UpdateId = updateId;
        }

        public int UpdateId { get; }

        public abstract IChatContext Chat { get; }
    }
}