using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace MotoHealth.Bot.AppInsights
{
    public class TelegramRequestTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IBotUpdateAccessor _botUpdateAccessor;

        public TelegramRequestTelemetryInitializer(IBotUpdateAccessor botUpdateAccessor)
        {
            _botUpdateAccessor = botUpdateAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (_botUpdateAccessor.Current != null && telemetry is RequestTelemetry requestTelemetry)
            {
                requestTelemetry.Name = "Telegram Webhook";
                requestTelemetry.Source = "Telegram";
            }
        }
    }
}