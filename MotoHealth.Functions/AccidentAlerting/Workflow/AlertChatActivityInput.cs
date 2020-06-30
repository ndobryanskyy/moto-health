using MotoHealth.Common.Dto;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public sealed class AlertChatActivityInput
    {
        public long ChatId { get; set; }

        public AccidentReportDto AccidentReport { get; set; } = default!;
    }
}