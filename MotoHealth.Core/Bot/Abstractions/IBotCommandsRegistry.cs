using MotoHealth.Core.Bot.Commands;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IBotCommandsRegistry
    {
        CommandDefinition Start { get; }

        CommandDefinition Info { get; }
        
        CommandDefinition ReportAccident { get; }

        ChatSubscriptionCommandDefinition SubscribeChat { get; }
        
        ChatSubscriptionCommandDefinition UnsubscribeChat { get; }

        BanCommandDefinition BanUser { get; }
        
        BanCommandDefinition UnbanUser { get; }

        CommandDefinition[] PublicCommands { get; }
    }
}