namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface ICommandBotUpdate : IMessageBotUpdate
    {
        BotCommand Command { get; }

        string[] Arguments { get; }
    }
}