using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.Extensions;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Telegram.Messages;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Bot.Middleware
{
    public sealed class BannedUsersHandlerMiddleware : IMiddleware
    {
        private static readonly IMessage HateMessage = MessageFactory.CreateTextMessage()
            .WithPlainText("https://youtu.be/h0ztJzMZbzM");

        private readonly ILogger<BannedUsersHandlerMiddleware> _logger;
        private readonly IBotTelemetryService _botTelemetryService;
        private readonly IUsersBanService _banService;

        public BannedUsersHandlerMiddleware(
            ILogger<BannedUsersHandlerMiddleware> logger,
            IBotTelemetryService botTelemetryService,
            IUsersBanService banService)
        {
            _logger = logger;
            _botTelemetryService = botTelemetryService;
            _banService = banService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var updateContext = context.GetChatUpdateContext();

            if (updateContext is { IsUpdateHandled: false, Update: { Chat: { Type: ChatType.Private }, Sender: var sender } })
            {
                var isUserBanned = await _banService.CheckIfUserIsBannedAsync(
                    sender.Id,
                    context.RequestAborted);

                if (isUserBanned)
                {
                    _logger.LogWarning("Message from banned chat");
                    _botTelemetryService.OnUpdateSkipped();

                    await updateContext.SendMessageAsync(HateMessage, context.RequestAborted);

                    updateContext.IsUpdateHandled = true;
                    return;
                }
            }

            await next(context);
        }
    }
}