using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatControllersFactory : IChatControllersFactory
    {
        public IChatController Create(long chatId) 
            => new ChatController(chatId);
    }
}