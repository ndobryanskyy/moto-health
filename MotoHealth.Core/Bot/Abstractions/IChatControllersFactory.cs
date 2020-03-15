namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatControllersFactory
    {
        IChatController Create(long chatId);
    }
}