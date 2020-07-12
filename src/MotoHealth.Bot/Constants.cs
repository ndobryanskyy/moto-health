namespace MotoHealth.Bot
{
    internal static class Constants
    {
        public static class Telegram
        {
            public const string ConfigurationSectionName = "Telegram";

            public const string WebhookPath = "/updates";

            public const string BotIdQueryParamName = "botId";
            
            public const string BotSecretQueryParamName = "botSecret";
        }

        public static class AzureStorage
        {
            public const string ConfigurationSectionName = "AzureStorage";
        }

        public static class ApplicationInsights
        {
            public const string ConfigurationSectionName = "ApplicationInsights";
            public const string AlwaysOnPingSyntheticSource = "Always On Ping";
        }

        public static class Authorization
        {
            public const string SecretsConfigurationSectionName = "Secrets";
        }
    }
}