using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatStatesRepository : IChatStatesRepository
    {
        private readonly ILogger<ChatStatesRepository> _logger;
        private readonly IChatStateInMemoryCache _cache;
        private readonly IChatStatesStore _store;

        public ChatStatesRepository(
            ILogger<ChatStatesRepository> logger,
            IChatStateInMemoryCache cache,
            IChatStatesStore store)
        {
            _logger = logger;
            _cache = cache;
            _store = store;
        }

        public async Task<IChatState> CreateForChatAsync(long chatId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Creating state for chat {chatId}");

            var state = await _store.CreateAsync(chatId, cancellationToken);
            _cache.CacheChatState(state);

            return state;
        }

        public async ValueTask<IChatState?> GetForChatAsync(long chatId, CancellationToken cancellationToken)
        {
            if (_cache.TryGetStateForChat(chatId, out var fromCache))
            {
                _logger.LogDebug($"Cache hit for {chatId}");

                return fromCache;
            }

            _logger.LogDebug($"Cache miss for {chatId}");

            var fromStore = await _store.GetByChatIdAsync(chatId, cancellationToken);

            if (fromStore != null)
            {
                _cache.CacheChatState(fromStore);
            }

            return fromStore;
        }

        public async Task UpdateAsync(IChatState state, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Updating state for chat {state.AssociatedChatId}");

            await _store.UpdateAsync(state, cancellationToken);
            _cache.CacheChatState(state);
        }
    }
}