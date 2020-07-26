using System;

namespace MotoHealth.Core.Bot.Commands
{
    public sealed class CommandName
    {
        private readonly string _name;

        private CommandName(string name)
        {
            if (!name.StartsWith("/") || name.Length < 2)
            {
                throw new ArgumentException("Command must start with '/' and have at least one letter");
            }

            _name = name;
        }

        public static implicit operator string(CommandName commandName) => commandName._name;

        public static implicit operator CommandName(string name) => new CommandName(name);
    }
}