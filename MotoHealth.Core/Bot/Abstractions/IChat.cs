using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChat
    {
        Task HandleUpdateAsync(IBotUpdate update, CancellationToken cancellationToken);
    }
}