namespace MotoHealth.Bot.Telegram.Updates
{
    public interface ICommandBotUpdate : IMessageBotUpdate
    {
        BotCommand Command { get; }
    }
}