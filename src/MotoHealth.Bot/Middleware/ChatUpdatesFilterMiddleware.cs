using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.Extensions;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Bot.Middleware
{
    public sealed class ChatUpdatesFilterMiddleware : IMiddleware
    {
        private readonly ILogger<ChatUpdatesFilterMiddleware> _logger;
        private readonly IBotTelemetryService _telemetryService;
        private readonly IChatFactory _chatFactory;

        public ChatUpdatesFilterMiddleware(
            ILogger<ChatUpdatesFilterMiddleware> logger,
            IBotTelemetryService telemetryService,
            IChatFactory chatFactory)
        {
            _logger = logger;
            _telemetryService = telemetryService;
            _chatFactory = chatFactory;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var botUpdate = context.GetBotUpdate();

            if (botUpdate is IChatUpdate chatUpdate)
            {
                _logger.LogDebug($"Update {botUpdate.UpdateId} is a chat update");

                var updateContext = _chatFactory.CreateUpdateContext(chatUpdate);
                context.SetChatUpdateContext(updateContext);

                await next(context);
            }
            else
            {
                _logger.LogInformation($"Skipping update {botUpdate.UpdateId} because it is not a chat update");
                _telemetryService.OnUpdateSkipped();
            }
        }
    }
}