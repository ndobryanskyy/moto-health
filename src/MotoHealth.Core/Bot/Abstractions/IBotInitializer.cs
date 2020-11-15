using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IBotInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}