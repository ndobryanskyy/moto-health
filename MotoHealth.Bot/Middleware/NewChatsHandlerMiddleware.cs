using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.Extensions;
using MotoHealth.Core.Bot.Abstractions;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Bot.Middleware
{
    public sealed class NewChatsHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<NewChatsHandlerMiddleware> _logger;
        private readonly IBotTelemetryService _botTelemetryService;
        private readonly IChatStatesRepository _chatStatesRepository;
        private readonly IDefaultChatStateFactory _defaultChatStateFactory;

        public NewChatsHandlerMiddleware(
            ILogger<NewChatsHandlerMiddleware> logger,
            IBotTelemetryService botTelemetryService,
            IChatStatesRepository chatStatesRepository,
            IDefaultChatStateFactory defaultChatStateFactory)
        {
            _logger = logger;
            _botTelemetryService = botTelemetryService;
            _chatStatesRepository = chatStatesRepository;
            _defaultChatStateFactory = defaultChatStateFactory;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var chatUpdateContext = context.GetChatUpdateContext();

            var existingState = await _chatStatesRepository.GetForChatAsync(chatUpdateContext.ChatId, context.RequestAborted);
            if (existingState == null)
            {
                var defaultState = _defaultChatStateFactory.CreateDefaultState(chatUpdateContext.ChatId);

                if (chatUpdateContext.Update.Chat.Type == ChatType.Private)
                {
                    defaultState.UserSubscribed = true;
                }

                await _chatStatesRepository.AddAsync(defaultState, context.RequestAborted);
                
                _logger.LogDebug($"New chat {chatUpdateContext.ChatId} started");
                _botTelemetryService.OnNewChatStarted();
            }

            await next(context);
        }
    }
}