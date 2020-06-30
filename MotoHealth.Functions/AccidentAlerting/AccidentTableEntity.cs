using System;
using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Common.Dto;

namespace MotoHealth.Functions.AccidentAlerting
{
    internal sealed class AccidentTableEntity : TableEntity
    {
        public const string EntityRowKey = "Details";

        public string Id { get; set; } = default!;

        public int ReporterTelegramUserId { get; set; }

        public string ReporterPhoneNumber { get; set; } = default!;

        public string? AccidentAddress { get; set; }

        public MapLocationDto? AccidentLocation { get; set; }

        public string AccidentParticipant { get; set; } = default!;

        public string AccidentVictims { get; set; } = default!;

        public DateTime ReportedAtUtc { get; set; }

        public DateTime HandledAtUtc { get; set; }

        public bool AnyChatAlerted { get; set; }

        public static AccidentTableEntity CreateFromReportDto(AccidentReportDto reportDto)
        {
            return new AccidentTableEntity
            {
                PartitionKey = reportDto.Id,
                RowKey = EntityRowKey,

                Id = reportDto.Id,
                ReporterTelegramUserId = reportDto.ReporterTelegramUserId,
                ReporterPhoneNumber = reportDto.ReporterPhoneNumber,
                AccidentAddress = reportDto.AccidentAddress,
                AccidentLocation = reportDto.AccidentLocation,
                AccidentParticipant = reportDto.AccidentParticipant,
                AccidentVictims = reportDto.AccidentVictims,
                ReportedAtUtc = reportDto.ReportedAtUtc,
            };
        }
    }
}