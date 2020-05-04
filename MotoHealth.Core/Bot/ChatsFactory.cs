using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using Telegram.Bot;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatsFactory : IChatsFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IChatsDoorman _chatsDoorman;
        private readonly ITelegramBotClient _botClient;
        private readonly IChatStatesRepository _chatStatesRepository;
        private readonly IDefaultChatStateFactory _defaultChatStateFactory;
        private readonly IChatUpdateHandler _chatUpdateHandler;
        private readonly IBotTelemetryService _botTelemetryService;

        public ChatsFactory(
            ILoggerFactory loggerFactory,
            IChatsDoorman chatsDoorman,
            ITelegramBotClient botClient,
            IChatStatesRepository chatStatesRepository,
            IDefaultChatStateFactory defaultChatStateFactory,
            IChatUpdateHandler chatUpdateHandler,
            IBotTelemetryService botTelemetryService)
        {
            _loggerFactory = loggerFactory;
            _chatsDoorman = chatsDoorman;
            _botClient = botClient;
            _chatStatesRepository = chatStatesRepository;
            _defaultChatStateFactory = defaultChatStateFactory;
            _chatUpdateHandler = chatUpdateHandler;
            _botTelemetryService = botTelemetryService;
        }

        public IChat CreateChat(long chatId)
            => new Chat(
                chatId, 
                _loggerFactory,
                _chatsDoorman,
                _botClient,
                _chatStatesRepository,
                _defaultChatStateFactory,
                _chatUpdateHandler,
                _botTelemetryService
            );
    }
}