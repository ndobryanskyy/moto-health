namespace MotoHealth.Bot.Telegram.Updates
{
    internal abstract class MessageBotUpdate : BotUpdateBase, IMessageBotUpdate
    {
        protected MessageBotUpdate(int updateId, int messageId, IChatContext chat) 
            : base(updateId)
        {
            MessageId = messageId;
            Chat = chat;
        }

        public int MessageId { get; }

        public override IChatContext Chat { get; }
    }
}