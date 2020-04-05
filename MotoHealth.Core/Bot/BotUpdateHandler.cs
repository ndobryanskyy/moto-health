using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Core.Extensions;

namespace MotoHealth.Core.Bot
{
    internal sealed class BotUpdateHandler : IBotUpdateHandler
    {
        private readonly ILogger<BotUpdateHandler> _logger;
        private readonly IChatStatesRepository _statesRepository;
        private readonly IChatFactory _chatFactory;

        public BotUpdateHandler(
            ILogger<BotUpdateHandler> logger,
            IChatStatesRepository statesRepository,
            IChatFactory chatFactory)
        {
            _logger = logger;
            _statesRepository = statesRepository;
            _chatFactory = chatFactory;
        }

        public async Task HandleBotUpdateAsync(IBotUpdate update, CancellationToken cancellationToken)
        {
            var chatId = update.Chat.Id;

            var chatState = await _statesRepository.GetForChatAsync(chatId, cancellationToken);

            if (chatState == null)
            {
                _logger.LogInformation($"New chat started {chatId}");

                chatState = _chatFactory.CreateDefaultState(chatId);

                chatState.UserSubscribed = true;

                await _statesRepository.AddAsync(chatState, cancellationToken);
            }

            var clonedChatState = chatState.Clone();

            var updateContext = _chatFactory.CreateUpdateContext(update);
            var controller = _chatFactory.CreateController(updateContext, clonedChatState);

            _logger.LogDebug($"Handling update {update.UpdateId} for chat {chatId}");

            await controller.HandleUpdateAsync(cancellationToken);

            await _statesRepository.UpdateAsync(clonedChatState, cancellationToken);

            _logger.LogDebug($"Successfully handled update {update.UpdateId} for chat {chatId}");
        }
    }
}