﻿namespace MotoHealth.Bot
{
    public static class Constants
    {
        public static class AccidentsQueue
        {
            public const string ConnectionStringName = "AccidentsQueue";
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
    }
}