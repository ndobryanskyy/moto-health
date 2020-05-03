using System.Text.RegularExpressions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace MotoHealth.Bot.AppInsights
{
    internal sealed class TelegramDependencyTelemetryInitializer : ITelemetryInitializer
    {
        private readonly Regex _telegramUrlRegex = new Regex(@"https://api\.telegram\.org/bot(.+):(.+)/(.+)", RegexOptions.Compiled);

        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is DependencyTelemetry dependencyTelemetry)
            {
                var match = _telegramUrlRegex.Match(dependencyTelemetry.Data);
                if (match.Success)
                {
                    dependencyTelemetry.Type = "Telegram";
                    dependencyTelemetry.Target = "Bot API";
                    dependencyTelemetry.Name = match.Groups[3].Value;
                }
            }
        }
    }
}
