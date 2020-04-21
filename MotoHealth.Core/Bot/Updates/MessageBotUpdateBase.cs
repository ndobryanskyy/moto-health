using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public abstract class MessageBotUpdateBase : BotUpdateBase, IMessageBotUpdate
    {
        public int MessageId { get; internal set; }
    }
}