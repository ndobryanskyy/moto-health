using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MotoHealth.Bot.Filters
{
    internal sealed class IgnoreExceptionsFilter : IExceptionFilter
    {
        private readonly ILogger<IgnoreExceptionsFilter> _logger;

        public IgnoreExceptionsFilter(ILogger<IgnoreExceptionsFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Ignoring unhandled exception");

            context.Result = new OkResult();
            context.ExceptionHandled = true;
        }
    }
}