using MotoHealth.Bot.Telegram.Updates;

namespace MotoHealth.Bot
{
    internal interface IBotContextFactory
    {
        IBotUpdateContext CreateForUpdate(IBotController bot, IBotUpdate update);
    }

    internal sealed class BotContextFactory : IBotContextFactory
    {
        public IBotUpdateContext CreateForUpdate(IBotController bot, IBotUpdate update) 
            => new BotUpdateContext(update, bot.TelegramClient);
    }
}