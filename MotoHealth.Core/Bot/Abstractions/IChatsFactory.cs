namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatsFactory
    {
        IChat CreateChat(long chatId);
    }
}