using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public interface IAccidentsQueue
    {
        Task EnqueueReportAsync(AccidentReport report, CancellationToken cancellationToken);
    }
}