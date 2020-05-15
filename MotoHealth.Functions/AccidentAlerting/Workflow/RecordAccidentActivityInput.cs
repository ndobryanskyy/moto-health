using System;
using MotoHealth.PubSub.EventData;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public sealed class RecordAccidentActivityInput
    {
        public AccidentReportedEventData AccidentReport { get; set; } = default!;

        public DateTime ReportHandledAtUtc { get; set; }

        public bool AnyChatAlerted { get; set; }
    }
}