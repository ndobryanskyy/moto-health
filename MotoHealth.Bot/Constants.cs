namespace MotoHealth.Bot
{
    public static class Constants
    {
        public static class UpdatesQueue
        {
            public const string ConnectionStringName = "UpdatesQueue";
        }

        public static class Telegram
        {
            public const string ConfigurationSectionName = "Telegram";

            public const string BotIdQueryParamName = "botId";
            
            public const string BotSecretQueryParamName = "botSecret";
        }
    }
}