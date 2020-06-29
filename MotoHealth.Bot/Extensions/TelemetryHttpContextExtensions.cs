using System;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;

namespace MotoHealth.Bot.Extensions
{
    internal static class TelemetryHttpContextExtensions
    {
        public static RequestTelemetry GetRequestTelemetry(this HttpContext httpContext)
            => httpContext?.Features.Get<RequestTelemetry>() ?? throw new InvalidOperationException();
    }
}