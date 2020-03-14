using MotoHealth.Bot.Telegram;

namespace MotoHealth.Bot
{
    internal interface IBotFactory
    {
        IBotController CreateBot();
    }

    internal sealed class BotFactory : IBotFactory
    {
        private readonly ITelegramBotClientFactory _clientFactory;

        public BotFactory(ITelegramBotClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IBotController CreateBot() => new BotController(_clientFactory.CreateClient());
    }
}