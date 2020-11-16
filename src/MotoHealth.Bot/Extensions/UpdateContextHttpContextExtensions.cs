using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Bot.Extensions
{
    internal static class UpdateContextHttpContextExtensions
    {
        public static void SetBotUpdate(this HttpContext? httpContext, IBotUpdate update)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var existing = httpContext.Features.Get<IBotUpdate>();
            if (existing != null)
            {
                throw new InvalidOperationException();
            }

            httpContext.Features.Set(update);
        }

        public static bool TryGetBotUpdate(this HttpContext? httpContext, [NotNullWhen(true)] out IBotUpdate? botUpdate)
        {
            botUpdate = httpContext?.Features.Get<IBotUpdate>();
            return botUpdate != null;
        }

        public static IBotUpdate GetBotUpdate(this HttpContext? httpContext)
            => httpContext?.Features.Get<IBotUpdate>() ?? throw new InvalidOperationException();

        public static void SetChatUpdateContext(this HttpContext? httpContext, IChatUpdateContext updateContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var existing = httpContext.Features.Get<IChatUpdateContext>();
            if (existing != null)
            {
                throw new InvalidOperationException();
            }

            httpContext.Features.Set(updateContext);
        }

        public static bool TryGetChatUpdateContext(this HttpContext? httpContext, [NotNullWhen(true)] out IChatUpdateContext? updateContext)
        {
            updateContext = httpContext?.Features.Get<IChatUpdateContext>();
            return updateContext != null;
        }

        public static IChatUpdateContext GetChatUpdateContext(this HttpContext? httpContext) 
            => httpContext?.Features.Get<IChatUpdateContext>() ?? throw new InvalidOperationException();
    }
}