using System.Text.RegularExpressions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using MotoHealth.Telegram;

namespace MotoHealth.Bot.AppInsights
{
    internal sealed class TelegramDependencyTelemetryInitializer : ITelemetryInitializer
    {
        private static readonly Regex TelegramBotApiUrlRegex =
            new Regex(@$"{TelegramClientOptions.TelegramBotApiBaseUrl}(.+):(.+)/(.+)", RegexOptions.Compiled);

        private readonly ITelegramTelemetrySanitizer _telemetrySanitizer;

        public TelegramDependencyTelemetryInitializer(ITelegramTelemetrySanitizer telemetrySanitizer)
        {
            _telemetrySanitizer = telemetrySanitizer;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is DependencyTelemetry dependencyTelemetry)
            {
                var match = TelegramBotApiUrlRegex.Match(dependencyTelemetry.Data);
                if (match.Success)
                {
                    var methodName = match.Groups[3].Value;

                    dependencyTelemetry.Data = _telemetrySanitizer.SanitizeBotApiRequestUrl(dependencyTelemetry.Data);
                    dependencyTelemetry.Type = "Telegram";
                    dependencyTelemetry.Target = "Bot API";
                    dependencyTelemetry.Name = methodName;
                }
            }
        }
    }
}
