using System;
using Microsoft.Azure.Cosmos.Table;

namespace MotoHealth.Functions.AccidentAlerting
{
    public sealed class AccidentTableEntity : TableEntity
    {
        public const string EntityRowKey = "Details";

        public string Id { get; set; } = default!;

        public long ReporterTelegramUserId { get; set; }

        public string ReporterPhoneNumber { get; set; } = default!;

        public string? AccidentAddress { get; set; }

        public double? AccidentLocationLongitude { get; set; }
        
        public double? AccidentLocationLatitude { get; set; }

        public string AccidentParticipant { get; set; } = default!;

        public string AccidentVictims { get; set; } = default!;

        public DateTime ReportedAtUtc { get; set; }

        public DateTime ReportHandledAtUtc { get; set; }

        public bool AnyChatAlerted { get; set; }
    }
}