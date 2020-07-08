using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.Extensions;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Bot.Middleware
{
    public sealed class ReliableUpdateHandlingContextMiddleware : IMiddleware
    {
        private static readonly IMessage SomethingWentWrongMessage = MessageFactory
            .CreateTextMessage()
            .WithPlainText(
                "😥 Ой, что-то пошло не так\n\n" +
                "Попробуйте ещё раз, если проблема не пройдет, сообщите о ней, пожалуйста, @ndobryanskyy");

        private readonly ILogger<ReliableUpdateHandlingContextMiddleware> _logger;
        private readonly IBotTelemetryService _telemetryService;

        public ReliableUpdateHandlingContextMiddleware(
            ILogger<ReliableUpdateHandlingContextMiddleware> logger,
            IBotTelemetryService telemetryService)
        {
            _logger = logger;
            _telemetryService = telemetryService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                // So Telegram won't retry
                context.Response.StatusCode = StatusCodes.Status200OK;

                var hasBotUpdate = context.TryGetBotUpdate(out _);
                var hasChatUpdateContext = context.TryGetChatUpdateContext(out var chatUpdateContext);

                _logger.LogError(exception, "Error while handling update");

                if (hasBotUpdate)
                {
                    _telemetryService.OnUpdateHandlingFailed();
                }

                if (hasChatUpdateContext)
                {
                    await TrySendSomethingWentWrongMessage(chatUpdateContext!, context.RequestAborted);
                }
            }
        }

        private async Task TrySendSomethingWentWrongMessage(IChatUpdateContext updateContext, CancellationToken cancellationToken)
        {
            try
            {
                await updateContext.SendMessageAsync(SomethingWentWrongMessage, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to send 'Something Went Wrong' message");
            }
        }
    }
}