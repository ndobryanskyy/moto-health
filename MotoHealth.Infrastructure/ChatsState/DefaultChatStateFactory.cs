using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Infrastructure.ChatsState.Entities;

namespace MotoHealth.Infrastructure.ChatsState
{
    internal sealed class DefaultChatStateFactory : IDefaultChatStateFactory
    {
        public IChatState CreateDefaultState(long chatId)
            => new ChatState
            {
                AssociatedChatId = chatId
            };
    }
}