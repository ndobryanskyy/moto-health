using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Infrastructure.ChatStorage
{
    internal sealed class ChatStateInMemoryCache : IChatStateInMemoryCache
    {
        // TODO set from configuration
        private readonly TimeSpan _slidingExpirationTimeout = TimeSpan.FromHours(1);

        private readonly MemoryCache _cache;

        public ChatStateInMemoryCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public bool TryGetStateForChat(long chatId, [NotNullWhen(true)] out IChatState? state)
            => _cache.TryGetValue(chatId, out state);

        public void CacheChatState(IChatState state)
        {
            var options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_slidingExpirationTimeout);

            _cache.Set(state.AssociatedChatId, state, options);
        }
    }
}