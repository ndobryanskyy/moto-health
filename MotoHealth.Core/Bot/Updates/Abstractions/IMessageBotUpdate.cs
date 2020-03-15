namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface IMessageBotUpdate : IBotUpdate
    {
        int MessageId { get; }
    }
}