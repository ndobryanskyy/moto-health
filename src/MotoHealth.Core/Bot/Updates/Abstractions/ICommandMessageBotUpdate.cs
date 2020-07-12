namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface ICommandMessageBotUpdate : IMessageBotUpdate
    {
        string Command { get; }

        string Arguments { get; }
    }
}