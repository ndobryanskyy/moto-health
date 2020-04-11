using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class BotCommandsRegistry : IBotCommandsRegistry
    {
        public CommandDefinition Start { get; } = new CommandDefinition("/start");

        public CommandDefinition ReportAccident { get; } = new CommandDefinition("/dtp", "Сообщить о ДТП");
        
        public CommandDefinition About { get; } = new CommandDefinition("/info", "О нас");

        public CommandDefinition[] PublicCommands { get; }

        public BotCommandsRegistry()
        {
            PublicCommands = new[]
            {
                ReportAccident,
                About
            };
        }
    }
}