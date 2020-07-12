using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.Telegram;

namespace MotoHealth.Bot.Middleware
{
    public sealed class BotTokenVerificationMiddleware : IMiddleware
    {
        private readonly ILogger<BotTokenVerificationMiddleware> _logger;
        private readonly TelegramClientOptions _telegramClientOptions;

        public BotTokenVerificationMiddleware(
            ILogger<BotTokenVerificationMiddleware> logger,
            IOptions<TelegramClientOptions> telegramClientOptions)
        {
            _logger = logger;
            _telegramClientOptions = telegramClientOptions.Value;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var queryParams = context.Request.Query;

            var botId = queryParams[Constants.Telegram.BotIdQueryParamName];
            var botSecret = queryParams[Constants.Telegram.BotSecretQueryParamName];

            var tokenValid = botId == _telegramClientOptions.BotId && botSecret == _telegramClientOptions.BotSecret;

            if (tokenValid)
            {
                _logger.LogDebug("Bot token verification succeeded!");
                await next(context);
            }
            else
            {
                _logger.LogWarning("Bot token verification failed!\n" +
                                   $"Request had {nameof(botId)} = '{botId}' and {nameof(botSecret)} = '{botSecret}'");
                
                context.Response.StatusCode = StatusCodes.Status404NotFound;
            }
        }
    }
}