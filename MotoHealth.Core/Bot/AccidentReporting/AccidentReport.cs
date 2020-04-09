using System;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public sealed class AccidentReport
    {
        public AccidentReport(
            string dialogReferenceId,
            int reporterTelegramUserId, 
            DateTime reportedAtUtc, 
            string address, 
            string participants, 
            string victims, 
            string reporterPhoneNumber)
        {
            DialogReferenceId = dialogReferenceId;
            ReporterTelegramUserId = reporterTelegramUserId;
            ReportedAtUtc = reportedAtUtc;
            Address = address;
            Participants = participants;
            Victims = victims;
            ReporterPhoneNumber = reporterPhoneNumber;
        }

        public string DialogReferenceId { get; }

        public int ReporterTelegramUserId { get; }

        public string ReporterPhoneNumber { get; }

        public DateTime ReportedAtUtc { get; }

        public string Address { get; }

        public string Participants { get; }

        public string Victims { get; }
    }
}