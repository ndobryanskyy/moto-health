using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.AppInsights;
using MotoHealth.Bot.Filters;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Core.Telegram;
using Telegram.Bot.Types;

namespace MotoHealth.Bot.Controllers
{
    [Route("updates")]
    [TypeFilter(typeof(ValidBotTokenRequiredFilter), IsReusable = false)]
    [TypeFilter(typeof(IgnoreExceptionsFilter), IsReusable = false)]
    public sealed class TelegramUpdatesController : ControllerBase
    {
        private readonly ILogger<TelegramUpdatesController> _logger;
        private readonly IBotUpdatesMapper _updateMapper;
        private readonly IChatsFactory _chatsFactory;
        private readonly IBotTelemetryService _botTelemetryService;

        public TelegramUpdatesController(
            ILogger<TelegramUpdatesController> logger,
            IBotUpdatesMapper updateMapper,
            IChatsFactory chatsFactory,
            IBotTelemetryService botTelemetryService)
        {
            _logger = logger;
            _updateMapper = updateMapper;
            _chatsFactory = chatsFactory;
            _botTelemetryService = botTelemetryService;
        }

        [HttpPost]
        public async Task ReceiveWebHookAsync(
            [FromBody] Update update,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Received invalid update");

                return;
            }

            _logger.LogInformation($"Received telegram update: {update.Id}");

            var botUpdate = _updateMapper.MapTelegramUpdate(update);

            _logger.LogInformation($"Mapped Telegram update: {update.Id} to {botUpdate.GetUpdateTypeNameForTelemetry()}");

            _botTelemetryService.OnUpdateMapped(botUpdate);

            if (botUpdate is IChatUpdate chatUpdate)
            {
                var chat = _chatsFactory.CreateChat(chatUpdate.Chat.Id);
                await chat.HandleUpdateAsync(chatUpdate, cancellationToken);
            }
            else
            {
                _logger.LogInformation($"Skipping update {update.Id}");
                _botTelemetryService.OnUpdateSkipped();
            }
        }
    }
}