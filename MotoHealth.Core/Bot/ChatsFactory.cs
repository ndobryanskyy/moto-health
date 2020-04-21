using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatsFactory : IChatsFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IChatsDoorman _chatsDoorman;
        private readonly ITelegramBotClientFactory _botClientFactory;
        private readonly IChatStatesRepository _chatStatesRepository;
        private readonly IDefaultChatStateFactory _defaultChatStateFactory;
        private readonly IChatUpdateHandler _chatUpdateHandler;

        public ChatsFactory(
            ILoggerFactory loggerFactory,
            IChatsDoorman chatsDoorman,
            ITelegramBotClientFactory botClientFactory,
            IChatStatesRepository chatStatesRepository,
            IDefaultChatStateFactory defaultChatStateFactory,
            IChatUpdateHandler chatUpdateHandler)
        {
            _loggerFactory = loggerFactory;
            _chatsDoorman = chatsDoorman;
            _botClientFactory = botClientFactory;
            _chatStatesRepository = chatStatesRepository;
            _defaultChatStateFactory = defaultChatStateFactory;
            _chatUpdateHandler = chatUpdateHandler;
        }

        public IChat CreateChat(long chatId)
            => new Chat(
                chatId, 
                _loggerFactory,
                _chatsDoorman,
                _botClientFactory,
                _chatStatesRepository,
                _defaultChatStateFactory,
                _chatUpdateHandler
            );
    }
}