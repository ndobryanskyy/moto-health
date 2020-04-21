namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface ICommandBotUpdate : IMessageBotUpdate
    {
        string Command { get; }
    }
}