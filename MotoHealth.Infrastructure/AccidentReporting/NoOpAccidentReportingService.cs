using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Infrastructure.AccidentReporting
{
    internal sealed class NoOpAccidentReportingService : IAccidentReportingService
    {
        private readonly ILogger<IAccidentReportingService> _logger;

        public NoOpAccidentReportingService(
            ILogger<IAccidentReportingService> logger)
        {
            _logger = logger;
        }

        public Task ReportAccidentAsync(AccidentReport report, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Adding report dated {report.ReportedAtUtc:g} from {report.DialogReferenceId} dialog to queue");

            _logger.LogWarning($"Report dated {report.ReportedAtUtc:g} from {report.DialogReferenceId} dialog was handled by NO-OP service");

            return Task.CompletedTask;
        }
    }
}