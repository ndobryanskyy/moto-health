using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.Filters;
using MotoHealth.Bot.Telegram;
using MotoHealth.Core.Bot.Abstractions;
using Telegram.Bot.Types;

namespace MotoHealth.Bot.Controllers
{
    [Route("updates")]
    [TypeFilter(typeof(ValidBotTokenRequiredFilter), IsReusable = false)]
    [TypeFilter(typeof(IgnoreExceptionsFilter), IsReusable = false)]
    public sealed class TelegramUpdatesController : ControllerBase
    {
        private readonly ILogger<TelegramUpdatesController> _logger;
        private readonly IBotUpdateResolver _updateResolver;
        private readonly IChatsFactory _chatsFactory;

        public TelegramUpdatesController(
            ILogger<TelegramUpdatesController> logger,
            IBotUpdateResolver updateResolver,
            IChatsFactory chatsFactory)
        {
            _logger = logger;
            _updateResolver = updateResolver;
            _chatsFactory = chatsFactory;
        }

        [HttpPost]
        public async Task ReceiveWebHookAsync(
            [FromBody] Update update,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Received incorrect update:\n{ModelState}");
                return;
            }

            _logger.LogDebug($"Received telegram update: {update.Id}");

            if (_updateResolver.TryResolveSupportedUpdate(update, out var botUpdate))
            {
                _logger.LogInformation($"Update {update.Id} resolved to supported type: {update.Type}");

                var chat = _chatsFactory.CreateChat(botUpdate.Chat.Id);
                await chat.HandleUpdateAsync(botUpdate, cancellationToken);
            }
            else
            {
                _logger.LogInformation($"Skipping update {update.Id} of type {update.Type}");
            }
        }
    }
}