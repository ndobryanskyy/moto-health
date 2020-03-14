using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;

namespace MotoHealth.Bot
{
    internal interface IBotsInMemoryCache
    {
        bool TryGetForChat(long chatId, [NotNullWhen(true)] out IBotController? bot);

        void AddForChat(long chatId, IBotController bot);
    }

    internal sealed class BotsInMemoryCache : IBotsInMemoryCache
    {
        // TODO set from configuration
        private readonly TimeSpan _slidingExpirationTimeout = TimeSpan.FromHours(1);

        private readonly MemoryCache _cache;

        public BotsInMemoryCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public bool TryGetForChat(long chatId, out IBotController? bot) 
            => _cache.TryGetValue(chatId, out bot);

        public void AddForChat(long chatId, IBotController bot)
        {
            var options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_slidingExpirationTimeout);

            _cache.Set(chatId, bot, options);
        }
    }
}