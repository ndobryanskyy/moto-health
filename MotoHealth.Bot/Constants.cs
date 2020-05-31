namespace MotoHealth.Bot
{
    public static class Constants
    {
        public static class AzureEventGrid
        {
            public const string ConfigurationSectionName = "AzureEventGrid";
        }

        public static class Telegram
        {
            public const string ConfigurationSectionName = "Telegram";

            public const string BotIdQueryParamName = "botId";
            
            public const string BotSecretQueryParamName = "botSecret";
        }

        public static class ChatsStorage
        {
            public const string ConnectionStringName = "ChatsStorageAccount";
        }

        public static class ApplicationInsights
        {
            public const string AlwaysOnPingSyntheticSource = "Always On Ping";
        }
    }
}