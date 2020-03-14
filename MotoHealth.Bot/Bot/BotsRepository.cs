using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MotoHealth.Bot
{
    internal interface IBotsRepository
    {
        ValueTask<IBotController> GetBotForChatAsync(long chatId, CancellationToken cancellationToken);
    }

    internal sealed class BotsRepository : IBotsRepository
    {
        private readonly ILogger<BotsRepository> _logger;
        private readonly IBotsInMemoryCache _cache;
        private readonly IBotFactory _botFactory;

        public BotsRepository(
            ILogger<BotsRepository> logger,
            IBotsInMemoryCache cache,
            IBotFactory botFactory)
        {
            _logger = logger;
            _cache = cache;
            _botFactory = botFactory;
        }

        public async ValueTask<IBotController> GetBotForChatAsync(long chatId, CancellationToken cancellationToken)
        {
            if (_cache.TryGetForChat(chatId, out var cached))
            {
                _logger.LogDebug($"Got bot for chat {chatId} from cache");

                return cached;
            }

            _logger.LogDebug($"Creating bot for chat {chatId}");

            var created = _botFactory.CreateBot();

            _cache.AddForChat(chatId, created);

            _logger.LogDebug($"Bot for chat {chatId} added to cache");

            // TODO for testing purposes
            await Task.Delay(100, cancellationToken);

            return created;
        }
    }
}