namespace MotoHealth.Functions
{
    internal static class FunctionNames
    {
        public const string AdminBotUpdatesHandler = "AdminBotUpdatesHandler";
        public const string AccidentReportedEventHandler = "AccidentReportedEventHandler";

        public static class AccidentAlerting
        {
            public const string Workflow = "AccidentAlerting_Workflow";
            public const string GetChatSubscriptionsActivity = "AccidentAlerting_RetrieveChatSubscriptionsActivity";
            public const string AlertChatActivity = "AccidentAlerting_AlertChatActivity";
            public const string RecordAccidentActivity = "AccidentAlerting_RecordAccidentActivity";
        }
    }
}