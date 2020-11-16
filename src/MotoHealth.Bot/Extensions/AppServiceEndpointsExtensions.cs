using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MotoHealth.Bot.Extensions
{
    internal static class AppServiceEndpointsExtensions
    {
        public static IEndpointRouteBuilder MapAlwaysOnPing(this IEndpointRouteBuilder endpoints)
        {
            endpoints
                .Map("/", OnAlwaysOnPingAsync)
                .WithDisplayName("Always On Ping");

            return endpoints;
        }

        private static Task OnAlwaysOnPingAsync(HttpContext context)
        {
            var logger = context.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("AlwaysOn");

            var requestTelemetry = context.GetRequestTelemetry();
            requestTelemetry.Context.Operation.SyntheticSource = Constants.ApplicationInsights.AlwaysOnPingSyntheticSource;

            context.Response.StatusCode = StatusCodes.Status200OK;

            logger.LogDebug("Always On Ping Received");

            return Task.CompletedTask;
        }
    }
}