using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatController
    {
        Task HandleUpdateAsync(
            IAccidentReportDialogHandler accidentReportDialogHandler, 
            CancellationToken cancellationToken);
    }
}