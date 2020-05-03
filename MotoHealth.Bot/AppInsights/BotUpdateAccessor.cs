using System;
using Microsoft.AspNetCore.Http;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Bot.AppInsights
{
    public interface IBotUpdateAccessor
    {
        IBotUpdate? Current { get; }

        void Set(IBotUpdate botUpdate);
    }

    internal sealed class BotUpdateAccessor : IBotUpdateAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BotUpdateAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IBotUpdate? Current 
            => _httpContextAccessor.HttpContext?.Features.Get<IBotUpdate>();

        public void Set(IBotUpdate botUpdate)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                throw new InvalidOperationException();
            }

            var exists = httpContext.Features.Get<IBotUpdate>() != null;
            if (exists)
            {
                throw new InvalidOperationException();
            }

            httpContext.Features.Set(botUpdate);
        }
    }
}