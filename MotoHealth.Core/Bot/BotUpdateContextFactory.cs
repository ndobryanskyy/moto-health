using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class BotUpdateContextFactory : IBotUpdateContextFactory
    {
        public IBotUpdateContext CreateForUpdate(IChatController controller, IBotUpdate update) 
            => new BotUpdateContext(update, controller.TelegramClient);
    }
}