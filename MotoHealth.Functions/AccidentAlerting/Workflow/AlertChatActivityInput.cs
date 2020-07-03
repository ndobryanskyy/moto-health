namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public sealed class AlertChatActivityInput
    {
        public long ChatId { get; set; }

        public AccidentAlertingWorkflowInput.AccidentReportSummary AccidentReport { get; set; } = default!;
    }
}