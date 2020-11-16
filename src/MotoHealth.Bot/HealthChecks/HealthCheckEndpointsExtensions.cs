using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MotoHealth.Bot.HealthChecks
{
    internal static class HealthCheckEndpointsExtensions
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };

        public static IEndpointRouteBuilder MapHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = WriteHealthCheckResponseAsync
            });

            return endpoints;
        }

        private static async Task WriteHealthCheckResponseAsync(HttpContext httpContext, HealthReport healthReport)
        {
            var mapper = httpContext.RequestServices.GetRequiredService<IMapper>();
            var viewModel = mapper.Map<ApplicationHealthReportViewModel>(healthReport);

            var responseBody = httpContext.Response.Body;

            await JsonSerializer.SerializeAsync(
                responseBody,
                viewModel,
                cancellationToken: httpContext.RequestAborted,
                options: SerializerOptions);
        }
    }
}