using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace MotoHealth.Bot.AppInsights
{
    internal sealed class AzureTableDependencyTelemetryInitializer : ITelemetryInitializer
    {
        private const string AzureTableDependencyType = "Azure table";

        private static readonly string NotFoundResultCode = StatusCodes.Status404NotFound.ToString();
        private static readonly string ConflictResultCode = StatusCodes.Status409Conflict.ToString();

        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is DependencyTelemetry { Type: AzureTableDependencyType, Success: false } azureTableDependencyFailedTelemetry)
            {
                if (azureTableDependencyFailedTelemetry.ResultCode == NotFoundResultCode || azureTableDependencyFailedTelemetry.ResultCode == ConflictResultCode)
                {
                    azureTableDependencyFailedTelemetry.Success = true;
                    azureTableDependencyFailedTelemetry.Properties.Add("Success Changed To", "true");
                }
            }
        }
    }
}