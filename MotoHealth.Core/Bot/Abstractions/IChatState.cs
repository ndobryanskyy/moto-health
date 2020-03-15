namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatState
    {
        long AssociatedChatId { get; }

        bool UserSubscribed { get; set; }
    }
}