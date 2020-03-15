using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatControllersRepository : IChatControllersRepository
    {
        private readonly ILogger<ChatControllersRepository> _logger;
        private readonly IChatControllersInMemoryCache _cache;
        private readonly IChatControllersStore _store;

        public ChatControllersRepository(
            ILogger<ChatControllersRepository> logger,
            IChatControllersInMemoryCache cache,
            IChatControllersStore store)
        {
            _logger = logger;
            _cache = cache;
            _store = store;
        }

        public async ValueTask<IChatController?> GetForChatAsync(long chatId, CancellationToken cancellationToken)
        {
            if (_cache.TryGetForChat(chatId, out var fromCache))
            {
                _logger.LogDebug($"Cache hit for {chatId}");

                return fromCache;
            }

            _logger.LogInformation($"Cache miss for {chatId}");

            var fromStore = await _store.GetByChatIdAsync(chatId, cancellationToken);

            if (fromStore != null)
            {
                _cache.Add(fromStore);
            }

            return fromStore;
        }

        public async Task AddAsync(IChatController controller, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Adding new controller for chat {controller.ChatId}");

            await _store.SaveControllerAsync(controller, cancellationToken);
            _cache.Add(controller);
        }
    }
}