using System;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public sealed class AccidentReport
    {
        public AccidentReport(
            string id,
            int reporterTelegramUserId, 
            DateTime reportedAtUtc, 
            string accidentAddress, 
            string accidentParticipant, 
            string accidentVictims, 
            string reporterPhoneNumber)
        {
            Id = id;
            ReporterTelegramUserId = reporterTelegramUserId;
            ReportedAtUtc = reportedAtUtc;
            AccidentAddress = accidentAddress;
            AccidentParticipant = accidentParticipant;
            AccidentVictims = accidentVictims;
            ReporterPhoneNumber = reporterPhoneNumber;
        }

        public string Id { get; }

        public int ReporterTelegramUserId { get; }

        public string ReporterPhoneNumber { get; }

        public DateTime ReportedAtUtc { get; }

        public string AccidentAddress { get; }

        public string AccidentParticipant { get; }

        public string AccidentVictims { get; }
    }
}