namespace MotoHealth.Bot.Telegram.Updates
{
    public interface IMessageBotUpdate : IBotUpdate
    {
        int MessageId { get; }
    }
}