using System;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public sealed class RecordAccidentActivityInput
    {
        public AccidentAlertingWorkflowInput.AccidentReportSummary AccidentReport { get; set; } = default!;

        public DateTime ReportHandledAtUtc { get; set; }

        public bool AnyChatAlerted { get; set; }
    }
}