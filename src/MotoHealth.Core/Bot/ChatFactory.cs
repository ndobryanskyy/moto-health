using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram;

namespace MotoHealth.Core.Bot
{
    public interface IChatFactory
    {
        IChatUpdateContext CreateUpdateContext(IChatUpdate update);
    }

    internal sealed class ChatFactory : IChatFactory
    {
        private readonly ITelegramClient _telegramClient;
        private readonly IChatStatesRepository _chatStatesRepository;

        public ChatFactory(
            ITelegramClient telegramClient,
            IChatStatesRepository chatStatesRepository)
        {
            _telegramClient = telegramClient;
            _chatStatesRepository = chatStatesRepository;
        }

        public IChatUpdateContext CreateUpdateContext(IChatUpdate update) 
            => new ChatUpdateContext(update, _telegramClient, _chatStatesRepository);
    }
}