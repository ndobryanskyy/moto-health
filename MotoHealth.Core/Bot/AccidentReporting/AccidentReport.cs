using System;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public sealed class AccidentReport
    {
        public AccidentReport(
            string dialogReferenceId,
            int reporterTelegramUserId, 
            DateTime reportedAtUtc, 
            string accidentAddress, 
            string accidentParticipants, 
            string accidentVictims, 
            string reporterPhoneNumber)
        {
            DialogReferenceId = dialogReferenceId;
            ReporterTelegramUserId = reporterTelegramUserId;
            ReportedAtUtc = reportedAtUtc;
            AccidentAddress = accidentAddress;
            AccidentParticipants = accidentParticipants;
            AccidentVictims = accidentVictims;
            ReporterPhoneNumber = reporterPhoneNumber;
        }

        public string DialogReferenceId { get; }

        public int ReporterTelegramUserId { get; }

        public string ReporterPhoneNumber { get; }

        public DateTime ReportedAtUtc { get; }

        public string AccidentAddress { get; }

        public string AccidentParticipants { get; }

        public string AccidentVictims { get; }
    }
}