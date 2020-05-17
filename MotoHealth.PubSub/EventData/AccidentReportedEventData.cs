using System;
using Newtonsoft.Json;

namespace MotoHealth.PubSub.EventData
{
    public sealed class AccidentReportedEventData
    {
        public const string Version = "1";

        [JsonProperty(Required = Required.Always)]
        public string ReportId { get; set; } = default!;

        [JsonProperty(Required = Required.Always)]
        public int ReporterTelegramUserId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ReporterPhoneNumber { get; set; } = default!;

        [JsonProperty(Required = Required.Always)]
        public DateTime ReportedAtUtc { get; set; }

        public string? AccidentAddress { get; set; }

        public MapLocation? AccidentLocation { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string AccidentParticipant { get; set; } = default!;

        [JsonProperty(Required = Required.Always)]
        public string AccidentVictims { get; set; } = default!;
    }
}