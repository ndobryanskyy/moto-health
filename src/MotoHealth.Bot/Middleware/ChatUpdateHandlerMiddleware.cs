using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.Extensions;
using MotoHealth.Core.Bot.ChatUpdateHandlers;

namespace MotoHealth.Bot.Middleware
{
    public sealed class ChatUpdateHandlerMiddleware<TChatUpdateHandler>: IMiddleware where TChatUpdateHandler: ChatUpdateHandlerBase
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly TChatUpdateHandler _handler;

        public ChatUpdateHandlerMiddleware(ILoggerFactory loggerFactory, TChatUpdateHandler handler)
        {
            _loggerFactory = loggerFactory;
            _handler = handler;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var updateContext = context.GetChatUpdateContext();
            var handlerLogger = _loggerFactory.CreateLogger($"{typeof(TChatUpdateHandler).Name}:{updateContext.ChatId}:{updateContext.Update.UpdateId}");

            await _handler.HandleUpdateAsync(updateContext, handlerLogger, context.RequestAborted);

            await next(context);
        }
    }
}