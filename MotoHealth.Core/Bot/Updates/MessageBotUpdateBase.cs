using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public abstract class MessageBotUpdateBase : BotUpdateBase, IMessageBotUpdate
    {
        protected MessageBotUpdateBase(int updateId, int messageId, IChatContext chat) 
            : base(updateId)
        {
            MessageId = messageId;
            Chat = chat;
        }

        public int MessageId { get; }

        public override IChatContext Chat { get; }
    }
}