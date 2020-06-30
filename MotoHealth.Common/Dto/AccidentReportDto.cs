using System;
using Newtonsoft.Json;

namespace MotoHealth.Common.Dto
{
    public sealed class AccidentReportDto
    {
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; } = default!;

        [JsonProperty(Required = Required.Always)]
        public int ReporterTelegramUserId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ReporterPhoneNumber { get; set; } = default!;

        [JsonProperty(Required = Required.Always)]
        public DateTime ReportedAtUtc { get; set; }

        public string? AccidentAddress { get; set; }

        public MapLocationDto? AccidentLocation { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string AccidentParticipant { get; set; } = default!;

        [JsonProperty(Required = Required.Always)]
        public string AccidentVictims { get; set; } = default!;
    }
}