using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.Telegram;

namespace MotoHealth.Bot.Filters
{
    internal sealed class ValidBotTokenRequiredFilter : IResourceFilter
    {
        private readonly ILogger<ValidBotTokenRequiredFilter> _logger;
        private readonly TelegramOptions _telegramOptions;

        public ValidBotTokenRequiredFilter(
            ILogger<ValidBotTokenRequiredFilter> logger,
            IOptions<TelegramOptions> telegramOptions)
        {
            _logger = logger;
            _telegramOptions = telegramOptions.Value;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var queryParams = context.HttpContext.Request.Query;

            var botId = queryParams[Constants.Telegram.BotIdQueryParamName];
            var botSecret = queryParams[Constants.Telegram.BotSecretQueryParamName];

            var tokenValid = botId == _telegramOptions.BotId &&
                             botSecret == _telegramOptions.BotSecret;

            if (tokenValid)
            {
                _logger.LogDebug("Bot token verification succeeded!");
            }
            else
            {
                _logger.LogWarning("Bot token verification failed!");
                _logger.LogWarning($"Request had {nameof(botId)} = '{botId}' and {nameof(botSecret)} = '{botSecret}'");

                context.Result = new NotFoundResult();   
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // Does nothing
        }
    }
}