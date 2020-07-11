using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.Extensions;
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
        private readonly IChatStatesRepository _chatStatesRepository;

        public BannedUsersHandlerMiddleware(
            ILogger<BannedUsersHandlerMiddleware> logger,
            IBotTelemetryService botTelemetryService,
            IChatStatesRepository chatStatesRepository)
        {
            _logger = logger;
            _botTelemetryService = botTelemetryService;
            _chatStatesRepository = chatStatesRepository;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var updateContext = context.GetChatUpdateContext();

            if (!updateContext.IsUpdateHandled && 
                updateContext.Update.Chat.Type == ChatType.Private)
            {
                var state = await _chatStatesRepository.GetForChatAsync(updateContext.ChatId, context.RequestAborted)
                            ?? throw new InvalidOperationException();

                if (state.UserBanned)
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