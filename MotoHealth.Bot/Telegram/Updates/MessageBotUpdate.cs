namespace MotoHealth.Bot.Telegram.Updates
{
    internal abstract class MessageBotUpdate : BotUpdateBase
    {
        protected MessageBotUpdate(int updateId, IChatContext chat) 
            : base(updateId)
        {
            Chat = chat;
        }

        public override IChatContext Chat { get; }
    }
}