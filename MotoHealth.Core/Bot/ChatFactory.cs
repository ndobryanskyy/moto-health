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
        private readonly IBotCommandsRegistry _botCommandsRegistry;

        public ChatFactory(
            IDefaultChatStateFactory defaultChatStateFactory,
            ITelegramBotClientFactory botClientFactory,
            IMessageFactory messageFactory,
            IBotCommandsRegistry botCommandsRegistry)
        {
            _defaultChatStateFactory = defaultChatStateFactory;
            _botClientFactory = botClientFactory;
            _messageFactory = messageFactory;
            _botCommandsRegistry = botCommandsRegistry;
        }

        public IChatController CreateController(IBotUpdateContext updateContext, IChatState state) 
            => new ChatController(updateContext, state, _messageFactory, _botCommandsRegistry);

        public IChatState CreateDefaultState(long chatId)
            => _defaultChatStateFactory.CreateDefaultState(chatId);

        public IBotUpdateContext CreateUpdateContext(IBotUpdate update)
            => new BotUpdateContext(update, _botClientFactory.CreateClient());
    }
}