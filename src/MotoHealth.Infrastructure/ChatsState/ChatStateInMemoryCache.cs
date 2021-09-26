using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Infrastructure.ChatsState
{
    internal sealed class ChatStateInMemoryCache : IChatStateInMemoryCache
    {
        // TODO move to configuration
        private static readonly TimeSpan SlidingExpirationTimeout = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan ExpirationScanFrequency = TimeSpan.FromMinutes(5);

        private readonly MemoryCache _cache;

        public ChatStateInMemoryCache()
        {
            var cacheOptions = new MemoryCacheOptions
            {
                ExpirationScanFrequency = ExpirationScanFrequency
            };

            _cache = new MemoryCache(cacheOptions);
        }

        public bool TryGetStateForChat(long chatId, [NotNullWhen(true)] out IChatState? state)
            => _cache.TryGetValue(chatId, out state!);

        public void CacheChatState(IChatState state)
        {
            var options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(SlidingExpirationTimeout);

            _cache.Set(state.AssociatedChatId, state, options);
        }
    }
}