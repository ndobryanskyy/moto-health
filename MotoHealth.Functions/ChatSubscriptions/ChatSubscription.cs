namespace MotoHealth.Functions.ChatSubscriptions
{
    public class ChatSubscription
    {
        public ChatSubscription(long chatId)
        {
            ChatId = chatId;
        }

        public long ChatId { get; }
    }
}