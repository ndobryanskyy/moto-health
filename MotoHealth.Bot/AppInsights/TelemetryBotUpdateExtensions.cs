using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Bot.AppInsights
{
    internal static class TelemetryBotUpdateExtensions
    {
        public static TelemetryProperties ExtractDiagnosticProperties(this IBotUpdate botUpdate)
        {
            var properties = new TelemetryProperties
            {
                { TelemetryProperties.WellKnown.UpdateType, botUpdate.GetUpdateTypeNameForTelemetry() }
            };

            switch (botUpdate)
            {
                case INotMappedMessageBotUpdate notMappedMessageBotUpdate:
                    properties.Add(TelemetryProperties.WellKnown.OriginalUpdateType, notMappedMessageBotUpdate.OriginalUpdate.Type.ToString());
                    properties.Add("Original Message Type", notMappedMessageBotUpdate.OriginalMessage.Type.ToString());
                    
                    break;

                case INotMappedBotUpdate notMappedBotUpdate:
                    properties.Add(TelemetryProperties.WellKnown.OriginalUpdateType, notMappedBotUpdate.OriginalUpdate.Type.ToString());

                    break;

                case ICommandBotUpdate commandBotUpdate:
                    properties.Add(TelemetryProperties.WellKnown.Command, commandBotUpdate.Command);

                    break;

                case ITextMessageBotUpdate textBotUpdate:
                    properties.Add(TelemetryProperties.WellKnown.MessageText, textBotUpdate.Text);

                    break;
            }

            return properties;
        }

        public static string GetUpdateTypeNameForTelemetry(this IBotUpdate botUpdate)
            => botUpdate.GetType().Name;
    }
}