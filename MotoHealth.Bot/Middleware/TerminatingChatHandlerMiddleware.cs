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
    public sealed class TerminatingChatHandlerMiddleware : IMiddleware
    {
        private static readonly IMessage PleaseSelectCommandMessage = MessageFactory.CreateTextMessage()
            .WithHtml("Пожалуйста, выберите команду в меню <b>[ / ]</b> внизу");

        private static readonly IMessage NothingToSayMessage = MessageFactory.CreateCompositeMessage()
            .AddMessage(CommonMessages.NotQuiteGetIt)
            .AddMessage(PleaseSelectCommandMessage);

        private readonly ILoggerFactory _loggerFactory;
        private readonly IBotTelemetryService _telemetryService;
        private readonly IChatStatesRepository _chatStatesRepository;

        public TerminatingChatHandlerMiddleware(
            ILoggerFactory loggerFactory,
            IBotTelemetryService telemetryService,
            IChatStatesRepository chatStatesRepository)
        {
            _loggerFactory = loggerFactory;
            _telemetryService = telemetryService;
            _chatStatesRepository = chatStatesRepository;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var updateContext = context.GetChatUpdateContext();
            var logger = _loggerFactory.CreateLogger($"{nameof(TerminatingChatHandlerMiddleware)}:{updateContext.ChatId}:{updateContext.Update.UpdateId}");

            if (!updateContext.IsUpdateHandled)
            {
                logger.LogDebug("Update was not handled");
                _telemetryService.OnNothingToSay();

                if (updateContext.Update.Chat.Type == ChatType.Private)
                {
                    await updateContext.SendMessageAsync(NothingToSayMessage, context.RequestAborted);
                }
            }
            else
            {
                var state = await updateContext.GetStagingStateAsync(context.RequestAborted);
                await _chatStatesRepository.UpdateAsync(state, context.RequestAborted);
            }

            _telemetryService.OnUpdateHandled();
        }
    }
}