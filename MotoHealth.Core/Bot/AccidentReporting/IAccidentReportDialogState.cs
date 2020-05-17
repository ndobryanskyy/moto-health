using System;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public interface IAccidentReportDialogState : IWaterfallDialogState
    {
        public string ReportId { get; }

        public DateTimeOffset StartedAt { get; }

        public string? Address { get; set; }

        public IMapLocation? Location { get; set; }

        public string Participant { get; set; }

        public string Victims { get; set; }

        public string ReporterPhoneNumber { get; set; }
    }
}