using System;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public sealed class AccidentReport
    {
        private AccidentReport()
        {
        }

        public string Id { get; set; } = default!;

        public int ReporterTelegramUserId { get; set; }

        public DateTime ReportedAtUtc { get; set; }

        public string ReporterPhoneNumber { get; set; } = default!;

        public string? AccidentAddress { get; set; }

        public IMapLocation? AccidentLocation { get; set; }

        public string AccidentParticipant { get; set; } = default!;

        public string AccidentVictims { get; set; } = default!;

        public static AccidentReport CreateFromDialogState(IAccidentReportingDialogState dialogState)
        {
            if (dialogState.Address == null && dialogState.Location == null)
            {
                throw new InvalidOperationException("Address and location should not be null at the same time");
            }

            return new AccidentReport
            {
                Id = dialogState.ReportId,
                ReporterPhoneNumber = dialogState.ReporterPhoneNumber,
                AccidentAddress = dialogState.Address,
                AccidentLocation = dialogState.Location,
                AccidentParticipant = dialogState.Participant,
                AccidentVictims = dialogState.Victims
            };
        }
    }
}