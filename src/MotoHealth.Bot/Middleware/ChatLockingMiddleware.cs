using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MotoHealth.Bot.Exceptions;
using MotoHealth.Bot.Extensions;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Bot.Middleware
{
    public sealed class ChatLockingMiddleware : IMiddleware
    {
        private readonly IChatsDoorman _doorman;

        public ChatLockingMiddleware(IChatsDoorman doorman)
        {
            _doorman = doorman;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var chatId = context.GetChatUpdateContext().ChatId;

            if (_doorman.TryLockChat(chatId, out var chatLock))
            {
                using (chatLock)
                {
                    await next(context);
                }
            }
            else
            {
                throw new ChatIsLockedException(chatId);
            }
        }
    }
}