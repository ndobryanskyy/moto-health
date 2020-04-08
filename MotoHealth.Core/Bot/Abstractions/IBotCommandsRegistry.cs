namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IBotCommandsRegistry
    {
        CommandDefinition Start { get; }

        CommandDefinition ReportAccident { get; }

        CommandDefinition[] PublicCommands { get; }
    }
}