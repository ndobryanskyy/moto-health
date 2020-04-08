using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public sealed class CommandBotUpdate : MessageBotUpdateBase, ICommandBotUpdate
    {
        public CommandBotUpdate(
            int updateId,
            int messageId,
            TelegramChat chat, 
            string command, 
            string[] arguments)
            : base(updateId, messageId, chat)
        {
            Command = command;
            Arguments = arguments;
        }
        
        public string Command { get; }

        public string[] Arguments { get; }
    }
}