using System;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public sealed class AccidentReport
    {
        public AccidentReport(
            string id,
            DateTime reportedAtUtc,
            AccidentReporter reporter,
            AccidentDetails accident)
        {
            Id = id;
            ReportedAtUtc = reportedAtUtc;
            Reporter = reporter;
            Accident = accident;
        }

        public string Id { get; }

        public DateTime ReportedAtUtc { get; }

        public AccidentReporter Reporter { get; }

        public AccidentDetails Accident { get; }
    }
}