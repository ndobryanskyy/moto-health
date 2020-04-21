using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Messages;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Core.Extensions;
using Telegram.Bot;

namespace MotoHealth.Core.Bot
{
    internal sealed class Chat : IChat
    {
        private static readonly IMessage SomethingWentWrongMessage = MessageFactory
            .CreateTextMessage()
            .WithPlainText("😥 Извините, но что-то пошло не так\n\nПопробуйте ещё раз, если проблема не пройдет, сообщите о ней, пожалуйста, @ndobryanskyy");

        private readonly long _chatId;

        private readonly ILogger _logger;
        private readonly IChatsDoorman _chatsDoorman;
        private readonly ITelegramBotClient _botClient;
        private readonly IChatStatesRepository _chatStatesRepository;
        private readonly IDefaultChatStateFactory _defaultChatStateFactory;
        private readonly IChatUpdateHandler _chatUpdateHandler;

        public Chat(
            long chatId,
            ILoggerFactory loggerFactory,
            IChatsDoorman chatsDoorman,
            ITelegramBotClientFactory botClientFactory,
            IChatStatesRepository chatStatesRepository,
            IDefaultChatStateFactory defaultChatStateFactory,
            IChatUpdateHandler chatUpdateHandler)
        {
            _chatId = chatId;

            _logger = loggerFactory.CreateLogger($"{nameof(Chat)}:{_chatId}");
            _chatsDoorman = chatsDoorman;
            _botClient = botClientFactory.CreateClient();
            _chatStatesRepository = chatStatesRepository;
            _defaultChatStateFactory = defaultChatStateFactory;
            _chatUpdateHandler = chatUpdateHandler;

            if (_chatUpdateHandler is ChatUpdateHandlerBase baseHandler)
            {
                baseHandler.Chat = this;
            }
        }

        public async Task HandleUpdateAsync(IBotUpdate update, CancellationToken cancellationToken)
        {
            if (update.Chat.Id != _chatId)
            {
                throw new ArgumentException($"Cannot handle update ({update.UpdateId}) from other chat {update.Chat.Id}", nameof(update));
            }

            _logger.LogInformation($"Start handling update {update.UpdateId}");

            var updateContext = new ChatUpdateContext(update, _botClient);

            try
            {
                await _chatUpdateHandler.HandleUpdateAsync(updateContext, cancellationToken);

                _logger.LogInformation($"Successfully finished handling update {update.UpdateId}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unhandled exception occured, while handling update {update.UpdateId}");

                await TrySendSomethingWentWrongMessage(updateContext, cancellationToken);
            }
        }

        public bool TryLock(out IDisposable? chatLock)
            => _chatsDoorman.TryLockChat(_chatId, out chatLock);

        public async ValueTask<IChatState> GetStateAsync(CancellationToken cancellationToken)
        {
            var state = await _chatStatesRepository.GetForChatAsync(_chatId, cancellationToken);

            if (state == null)
            {
                _logger.LogInformation("No previous state found");

                state = _defaultChatStateFactory.CreateDefaultState(_chatId);
                state.UserSubscribed = true;

                await _chatStatesRepository.AddAsync(state, cancellationToken);

                _logger.LogInformation("New state created");
            }

            return state.Clone();
        }

        public async Task UpdateStateAsync(IChatState state, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting state update");

            await _chatStatesRepository.UpdateAsync(state, cancellationToken);

            _logger.LogDebug("Finished state update");
        }

        private async Task TrySendSomethingWentWrongMessage(IChatUpdateContext updateContext, CancellationToken cancellationToken)
        {
            try
            {
                await updateContext.SendMessageAsync(SomethingWentWrongMessage, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to send 'Something Went Wrong' message for update: {updateContext.Update.UpdateId}");
            }
        }
    }
}