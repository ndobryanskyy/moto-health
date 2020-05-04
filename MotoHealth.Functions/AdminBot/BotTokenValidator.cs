using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.Telegram;

namespace MotoHealth.Functions.AdminBot
{
    public interface IBotTokenValidator
    {
        bool ValidateToken(HttpRequest request);
    }

    internal sealed class BotTokenValidator : IBotTokenValidator
    {
        private readonly ILogger<BotTokenValidator> _logger;
        private readonly TelegramOptions _telegramOptions;

        public BotTokenValidator(
            ILogger<BotTokenValidator> logger,
            IOptions<TelegramOptions> options)
        {
            _logger = logger;
            _telegramOptions = options.Value;
        }

        public bool ValidateToken(HttpRequest request)
        {
            var queryParams = request.Query;

            var botId = queryParams["botId"];
            var botSecret = queryParams["botSecret"];

            var tokenValid = botId == _telegramOptions.BotId &&
                             botSecret == _telegramOptions.BotSecret;

            if (tokenValid)
            {
                _logger.LogDebug("Bot token verification succeeded!");
            }
            else
            {
                _logger.LogWarning($"Bot token verification failed for botId = '{botId}' and botSecret = '{botSecret}'");
            }

            return tokenValid;
        }
    }
}