using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    internal sealed class CommandMessageBotUpdate : MessageBotUpdateBase, ICommandMessageBotUpdate
    {
        public string Command { get; set; } = default!;

        public string Arguments { get; set; } = default!;
    }
}