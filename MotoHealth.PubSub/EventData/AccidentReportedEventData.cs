using System;

namespace MotoHealth.PubSub.EventData
{
    public sealed class AccidentReportedEventData
    {
        public const string Version = "1";

        public string DialogReferenceId { get; set; } = default!;

        public int ReporterTelegramUserId { get; set; }

        public string ReporterPhoneNumber { get; set; } = default!;

        public DateTime ReportedAtUtc { get; set; }

        public string AccidentAddress { get; set; } = default!;

        public string AccidentParticipants { get; set; } = default!;

        public string AccidentVictims { get; set; } = default!;
    }
}