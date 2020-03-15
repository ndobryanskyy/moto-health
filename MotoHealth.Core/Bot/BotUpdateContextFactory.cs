using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot;

namespace MotoHealth.Core.Bot
{
    internal sealed class BotUpdateContextFactory : IBotUpdateContextFactory
    {
        private readonly ITelegramBotClient _client;

        public BotUpdateContextFactory(ITelegramBotClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient();
        }

        public IBotUpdateContext CreateForUpdate(IBotUpdate update) 
            => new BotUpdateContext(update, _client);
    }
}