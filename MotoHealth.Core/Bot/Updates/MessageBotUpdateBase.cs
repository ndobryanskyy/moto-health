using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public abstract class MessageBotUpdateBase : BotUpdateBase, IMessageBotUpdate
    {
        protected MessageBotUpdateBase(
            int updateId,
            int messageId, 
            TelegramChat chat) 
            : base(updateId)
        {
            MessageId = messageId;
            Chat = chat;
        }

        public int MessageId { get; }

        public override ITelegramChat Chat { get; }
    }
}