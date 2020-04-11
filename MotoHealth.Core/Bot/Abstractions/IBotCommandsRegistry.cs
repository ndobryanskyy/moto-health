namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IBotCommandsRegistry
    {
        CommandDefinition Start { get; }

        CommandDefinition About { get; }
        
        CommandDefinition ReportAccident { get; }

        CommandDefinition[] PublicCommands { get; }
    }
}