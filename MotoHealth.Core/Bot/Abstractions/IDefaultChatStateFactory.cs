namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IDefaultChatStateFactory
    {
        IChatState CreateDefaultState(long chatId);
    }
}