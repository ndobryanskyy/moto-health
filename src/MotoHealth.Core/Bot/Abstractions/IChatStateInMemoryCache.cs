using System.Diagnostics.CodeAnalysis;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatStateInMemoryCache
    {
        bool TryGetStateForChat(long chatId, [NotNullWhen(true)] out IChatState? state);

        void CacheChatState(IChatState state);
    }
}