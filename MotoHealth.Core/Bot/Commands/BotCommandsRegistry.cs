using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot.Commands
{
    internal sealed class BotCommandsRegistry : IBotCommandsRegistry
    {
        public CommandDefinition Start { get; } = new CommandDefinition("/start");

        public CommandDefinition ReportAccident { get; } = new CommandDefinition("/dtp", "Сообщить о ДТП");
        
        public CommandDefinition Info { get; } = new CommandDefinition("/info", "О нас");

        public ChatSubscriptionCommandDefinition SubscribeChat { get; } = new ChatSubscriptionCommandDefinition("/subscribe");

        public ChatSubscriptionCommandDefinition UnsubscribeChat { get; } = new ChatSubscriptionCommandDefinition("/unsubscribe");

        public BanCommandDefinition BanUser { get; } = new BanCommandDefinition("/ban");

        public BanCommandDefinition UnbanUser { get; } = new BanCommandDefinition("/unban");

        public CommandDefinition[] PublicCommands { get; }

        public BotCommandsRegistry()
        {
            PublicCommands = new[]
            {
                ReportAccident,
                Info
            };
        }
    }
}