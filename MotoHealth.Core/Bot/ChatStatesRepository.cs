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

        public async ValueTask<IChatState?> GetForChatAsync(long chatId, CancellationToken cancellationToken)
        {
            if (_cache.TryGetStateForChat(chatId, out var fromCache))
            {
                _logger.LogDebug($"Cache hit for {chatId}");

                return fromCache;
            }

            _logger.LogInformation($"Cache miss for {chatId}");

            var fromStore = await _store.GetByChatIdAsync(chatId, cancellationToken);

            if (fromStore != null)
            {
                _cache.CacheChatState(fromStore);
            }

            return fromStore;
        }

        public async Task AddAsync(IChatState state, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Adding state for chat {state.AssociatedChatId}");

            await _store.AddAsync(state, cancellationToken);
            _cache.CacheChatState(state);
        }
    }
}