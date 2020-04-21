using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public sealed class CommandMessageBotUpdate : MessageBotUpdateBase, ICommandBotUpdate
    {
        public string Command { get; internal set; } = string.Empty;
    }
}