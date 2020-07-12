using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public interface IAccidentReportingService
    {
        Task ReportAccidentAsync(AccidentReport report, CancellationToken cancellationToken);
    }
}