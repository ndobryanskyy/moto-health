namespace MotoHealth.Core.Bot.AccidentReporting
{
    public interface IAccidentReportDialogState : IWaterfallDialogState
    {
        public string Address { get; set; }

        public string Participants { get; set; }

        public string Victims { get; set; }

        public string? ReporterPhoneNumber { get; set; }
    }
}