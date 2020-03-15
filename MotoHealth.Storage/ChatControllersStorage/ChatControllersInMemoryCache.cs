using System;
using Microsoft.Extensions.Caching.Memory;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Infrastructure.ChatControllersStorage
{
    internal sealed class ChatControllersInMemoryCache : IChatControllersInMemoryCache
    {
        // TODO set from configuration
        private readonly TimeSpan _slidingExpirationTimeout = TimeSpan.FromHours(1);

        private readonly MemoryCache _cache;

        public ChatControllersInMemoryCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public bool TryGetForChat(long chatId, out IChatController? controller) 
            => _cache.TryGetValue(chatId, out controller);

        public void Add(IChatController controller)
        {
            var options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_slidingExpirationTimeout);

            _cache.Set(controller.ChatId, controller, options);
        }
    }
}