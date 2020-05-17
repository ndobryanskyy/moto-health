using System;
using Microsoft.Azure.Cosmos.Table;
using MotoHealth.PubSub.EventData;

namespace MotoHealth.Functions.AccidentAlerting
{
    internal sealed class AccidentTableEntity : TableEntity
    {
        public const string EntityRowKey = "Details";

        public string Id { get; set; } = default!;

        public int ReporterTelegramUserId { get; set; }

        public string ReporterPhoneNumber { get; set; } = default!;

        public string? AccidentAddress { get; set; }

        public MapLocation? AccidentLocation { get; set; }

        public string AccidentParticipant { get; set; } = default!;

        public string AccidentVictims { get; set; } = default!;

        public DateTime ReportedAtUtc { get; set; }

        public DateTime HandledAtUtc { get; set; }

        public bool AnyChatAlerted { get; set; }

        public static AccidentTableEntity CreateFromReportEventData(AccidentReportedEventData eventData)
        {
            return new AccidentTableEntity
            {
                PartitionKey = eventData.ReportId,
                RowKey = EntityRowKey,

                Id = eventData.ReportId,
                ReporterTelegramUserId = eventData.ReporterTelegramUserId,
                ReporterPhoneNumber = eventData.ReporterPhoneNumber,
                AccidentAddress = eventData.AccidentAddress,
                AccidentLocation = eventData.AccidentLocation,
                AccidentParticipant = eventData.AccidentParticipant,
                AccidentVictims = eventData.AccidentVictims,
                ReportedAtUtc = eventData.ReportedAtUtc,
            };
        }
    }
}