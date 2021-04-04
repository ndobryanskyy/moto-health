using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace MotoHealth.Bot.AppInsights
{
    internal sealed class AlwaysOnPingFilteringTelemetryProcessor : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public AlwaysOnPingFilteringTelemetryProcessor(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is RequestTelemetry { Context: { Operation: { SyntheticSource: Constants.ApplicationInsights.AlwaysOnPingSyntheticSource } } })
            {
                return;
            }

            _next.Process(item);
        }
    }
}