namespace MotoHealth.Bot.Telegram.Updates
{
    public interface ICommandBotUpdate : IBotUpdate
    {
        BotCommand Command { get; }

        string[] Arguments { get; }
    }
}