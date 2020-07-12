namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface IMessageBotUpdate : IChatUpdate
    {
        int MessageId { get; }
    }
}