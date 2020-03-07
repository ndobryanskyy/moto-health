using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.Authorization;
using MotoHealth.Bot.Messages;
using MotoHealth.Bot.Telegram;
using Telegram.Bot.Types;

namespace MotoHealth.Bot.Controllers
{
    [Route("updates")]
    [Authorize(Policy = Policy.BotTokenVerificationRequired)]
    public sealed class TelegramUpdatesController : ControllerBase
    {
        private readonly ILogger<TelegramUpdatesController> _logger;
        private readonly IBotUpdateResolver _updateResolver;
        private readonly ITelegramUpdatesQueue _updatesQueue;

        public TelegramUpdatesController(
            ILogger<TelegramUpdatesController> logger,
            IBotUpdateResolver updateResolver,
            ITelegramUpdatesQueue updatesQueue)
        {
            _logger = logger;
            _updateResolver = updateResolver;
            _updatesQueue = updatesQueue;
        }

        [HttpPost]
        public async Task ReceiveWebHookAsync(
            [FromBody] Update update,
            CancellationToken cancellationToken)
        {
            if (_updateResolver.TryResolveSupportedUpdate(update, out var botUpdate))
            {
                _logger.LogInformation($"Handling update of type: {update.Type}");

                await _updatesQueue.AddUpdateAsync(botUpdate);
            }
            else
            {
                _logger.LogInformation($"Skipping the update of type {update.Type}");
            }
        }
    }
}