using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Bot.Abstractions
{
    internal interface IConversationContext
    {
        Task SendTextMessageAsync(string text, CancellationToken cancellationToken);
    }
}