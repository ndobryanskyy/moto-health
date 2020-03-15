using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class BotUpdatesHandler : IBotUpdatesHandler
    {
        private readonly ILogger<BotUpdatesHandler> _logger;
        private readonly IChatControllersRepository _repository;
        private readonly IChatControllersFactory _chatControllersFactory;
        private readonly IBotUpdateContextFactory _botUpdateContextFactory;

        public BotUpdatesHandler(
            ILogger<BotUpdatesHandler> logger,
            IChatControllersRepository repository,
            IChatControllersFactory chatControllersFactory,
            IBotUpdateContextFactory botUpdateContextFactory)
        {
            _logger = logger;
            _repository = repository;
            _chatControllersFactory = chatControllersFactory;
            _botUpdateContextFactory = botUpdateContextFactory;
        }

        public async Task HandleBotUpdateAsync(IBotUpdate update, CancellationToken cancellationToken)
        {
            var chatId = update.Chat.Id;

            var chatController = await _repository.GetForChatAsync(chatId, cancellationToken);

            if (chatController == null)
            {
                _logger.LogInformation($"Creating controller for new chat {chatId}");

                chatController = _chatControllersFactory.Create(chatId);
                await _repository.AddAsync(chatController, cancellationToken);
            }

            var updateContext = _botUpdateContextFactory.CreateForUpdate(chatController, update);

            await chatController.HandleUpdateAsync(updateContext, cancellationToken);
        }
    }
}