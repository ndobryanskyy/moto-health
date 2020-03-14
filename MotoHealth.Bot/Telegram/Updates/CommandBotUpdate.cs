namespace MotoHealth.Bot.Telegram.Updates
{
    internal sealed class CommandBotUpdate : MessageBotUpdateBase, ICommandBotUpdate
    {
        public CommandBotUpdate(
            int updateId,
            int messageId,
            IChatContext chat, 
            BotCommand command, 
            string[] arguments)
            : base(updateId, messageId, chat)
        {
            Command = command;
            Arguments = arguments;
        }
        
        public BotCommand Command { get; }

        public string[] Arguments { get; }
    }
}