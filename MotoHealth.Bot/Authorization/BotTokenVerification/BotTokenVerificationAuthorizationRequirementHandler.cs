using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.Bot.Telegram;

namespace MotoHealth.Bot.Authorization
{
    internal sealed class BotTokenVerificationAuthorizationRequirementHandler : AuthorizationHandler<BotTokenVerificationAuthorizationRequirement>
    {
        private readonly ILogger<BotTokenVerificationAuthorizationRequirementHandler> _logger;
        private readonly TelegramOptions _telegramOptions;
        private readonly HttpContext _httpContext;

        public BotTokenVerificationAuthorizationRequirementHandler(
            ILogger<BotTokenVerificationAuthorizationRequirementHandler> logger,
            IOptions<TelegramOptions> telegramOptions,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _telegramOptions = telegramOptions.Value;
            _httpContext = httpContextAccessor.HttpContext;
        }

        protected override Task HandleRequirementAsync
            (AuthorizationHandlerContext context,
            BotTokenVerificationAuthorizationRequirement requirement)
        {
            var botId = _httpContext.Request.Query[Constants.Telegram.BotIdQueryParamName];
            var botSecret = _httpContext.Request.Query[Constants.Telegram.BotSecretQueryParamName];

            var tokenValid = botId == _telegramOptions.BotId && 
                                 botSecret == _telegramOptions.BotSecret;

            if (tokenValid)
            {
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning("Bot token verification failed!");
                _logger.LogWarning($"Request had {nameof(botId)} = {botId} and {nameof(botSecret)} = {botSecret}");

                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}