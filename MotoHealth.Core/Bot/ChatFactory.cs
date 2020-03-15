using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatFactory : IChatFactory
    {
        private readonly IDefaultChatStateFactory _defaultChatStateFactory;
        private readonly ITelegramBotClientFactory _botClientFactory;

        public ChatFactory(
            IDefaultChatStateFactory defaultChatStateFactory,
            ITelegramBotClientFactory botClientFactory)
        {
            _defaultChatStateFactory = defaultChatStateFactory;
            _botClientFactory = botClientFactory;
        }

        public IChatController CreateController(long chatId, IChatState state) 
            => new ChatController(chatId, state);

        public IChatState CreateDefaultState(long chatId)
            => _defaultChatStateFactory.CreateDefaultState(chatId);

        public IBotUpdateContext CreateUpdateContext(IBotUpdate update)
            => new BotUpdateContext(update, _botClientFactory.CreateClient());
    }
}