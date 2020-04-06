using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Messages;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatFactory : IChatFactory
    {
        private readonly IDefaultChatStateFactory _defaultChatStateFactory;
        private readonly ITelegramBotClientFactory _botClientFactory;
        private readonly IMessageFactory _messageFactory;

        public ChatFactory(
            IDefaultChatStateFactory defaultChatStateFactory,
            ITelegramBotClientFactory botClientFactory,
            IMessageFactory messageFactory)
        {
            _defaultChatStateFactory = defaultChatStateFactory;
            _botClientFactory = botClientFactory;
            _messageFactory = messageFactory;
        }

        public IChatController CreateController(IBotUpdateContext updateContext, IChatState state) 
            => new ChatController(updateContext, state, _messageFactory);

        public IChatState CreateDefaultState(long chatId)
            => _defaultChatStateFactory.CreateDefaultState(chatId);

        public IBotUpdateContext CreateUpdateContext(IBotUpdate update)
            => new BotUpdateContext(update, _botClientFactory.CreateClient());
    }
}