using System;
using MotoHealth.Common.Dto;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public sealed class RecordAccidentActivityInput
    {
        public AccidentReportDto AccidentReport { get; set; } = default!;

        public DateTime ReportHandledAtUtc { get; set; }

        public bool AnyChatAlerted { get; set; }
    }
}