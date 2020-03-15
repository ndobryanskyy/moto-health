using System.Diagnostics.CodeAnalysis;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatControllersInMemoryCache
    {
        bool TryGetForChat(long chatId, [NotNullWhen(true)] out IChatController? controller);

        void Add(IChatController controller);
    }
}