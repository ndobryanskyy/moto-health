using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IConversationContext
    {
        Task SendTextMessageAsync(string text, CancellationToken cancellationToken);
    }
}