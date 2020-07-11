using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Infrastructure.ChatsState
{
    internal sealed class ChatStateInMemoryCache : IChatStateInMemoryCache
    {
        // TODO set from configuration
        private readonly TimeSpan _slidingExpirationTimeout = TimeSpan.FromMinutes(15);

        private readonly MemoryCache _cache;

        public ChatStateInMemoryCache()
        {
            var cacheOptions = new MemoryCacheOptions
            {
                ExpirationScanFrequency = TimeSpan.FromMinutes(5)
            };

            _cache = new MemoryCache(cacheOptions);
        }

        public bool TryGetStateForChat(long chatId, [NotNullWhen(true)] out IChatState? state)
            => _cache.TryGetValue(chatId, out state!);

        public void CacheChatState(IChatState state)
        {
            var options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_slidingExpirationTimeout);

            _cache.Set(state.AssociatedChatId, state, options);
        }
    }
}