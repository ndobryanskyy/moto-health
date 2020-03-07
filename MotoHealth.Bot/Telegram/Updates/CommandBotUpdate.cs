using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;

namespace MotoHealth.Bot.Telegram.Updates
{
    internal sealed class CommandBotUpdate : MessageBotUpdate, ICommandBotUpdate
    {
        public CommandBotUpdate(Update update, BotCommand command, IEnumerable<string> arguments)
            : base(update)
        {
            Command = command;
            Arguments = arguments.ToArray();
        }
        
        public BotCommand Command { get; }

        public string[] Arguments { get; }
    }
}