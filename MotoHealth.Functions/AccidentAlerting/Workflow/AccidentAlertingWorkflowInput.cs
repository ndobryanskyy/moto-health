using System;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public class AccidentAlertingWorkflowInput
    {
        public long[] ChatsToNotify { get; set; } = default!;

        public AccidentReportSummary Report { get; set; } = default!;

        public sealed class AccidentReportSummary
        {
            public string Id { get; set; } = default!;

            public DateTime ReportedAtUtc { get; set; }

            public int ReporterTelegramUserId { get; set; }

            public string ReporterPhoneNumber { get; set; } = default!;

            public string? AccidentAddress { get; set; }

            public MapLocation? AccidentLocation { get; set; }

            public string AccidentParticipant { get; set; } = default!;

            public string AccidentVictims { get; set; } = default!;
        }

        public sealed class MapLocation
        {
            public double Latitude { get; set; }

            public double Longitude { get; set; }
        }
    }
}