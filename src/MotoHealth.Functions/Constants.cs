namespace MotoHealth.Functions
{
    internal static class Constants
    {
        public static class FunctionNames
        {
            public static class AccidentAlerting
            {
                public const string Trigger = "AccidentAlerting_WorkflowTrigger";
                public const string Workflow = "AccidentAlerting_Workflow";
                public const string AlertChatActivity = "AccidentAlerting_AlertChatActivity";
                public const string RecordAccidentActivity = "AccidentAlerting_RecordAccidentActivity";
            }
        }

        public static class Telegram
        {
            public const string ConfigurationSectionName = "Telegram";
        }

        public static class AzureStorage
        {
            public const string StorageAccountConnectionStringName = "StorageAccount";
        }
    }
}