using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    internal sealed class CommandMessageBotUpdate : MessageBotUpdateBase, ICommandBotUpdate
    {
        public string Command { get; set; } = default!;
    }
}