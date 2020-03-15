using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatControllersFactory : IChatControllersFactory
    {
        private readonly ITelegramBotClientFactory _clientFactory;

        public ChatControllersFactory(ITelegramBotClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IChatController Create(long chatId) 
            => new ChatController(chatId, _clientFactory.CreateClient());
    }
}