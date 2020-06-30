namespace MotoHealth.Bot
{
    public static class Constants
    {
        public static class Telegram
        {
            public const string ConfigurationSectionName = "Telegram";

            public const string BotIdQueryParamName = "botId";
            
            public const string BotSecretQueryParamName = "botSecret";
        }

        public static class AzureStorage
        {
            public const string ConfigurationSectionName = "AzureStorage";
        }

        public static class AppEventsTopic
        {
            public const string ConfigurationSectionName = "AppEventsTopic";
        }

        public static class ApplicationInsights
        {
            public const string AlwaysOnPingSyntheticSource = "Always On Ping";
        }

        public static class Authorization
        {
            public const string SecretsConfigurationSectionName = "Secrets";
        }
    }
}