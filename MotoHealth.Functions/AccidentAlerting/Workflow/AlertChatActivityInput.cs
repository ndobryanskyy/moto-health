using MotoHealth.PubSub.EventData;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public sealed class AlertChatActivityInput
    {
        public long ChatId { get; set; }

        public AccidentReportedEventData AccidentReport { get; set; } = default!;
    }
}