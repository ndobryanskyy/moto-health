using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace MotoHealth.Bot.AppInsights
{
    internal sealed class AlwaysOnPingFilteringTelemetryProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; }

        // next will point to the next TelemetryProcessor in the chain.
        public AlwaysOnPingFilteringTelemetryProcessor(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is RequestTelemetry { Context: { Operation: { SyntheticSource: Constants.ApplicationInsights.AlwaysOnPingSyntheticSource } } })
            {
                return;
            }

            Next.Process(item);
        }
    }
}