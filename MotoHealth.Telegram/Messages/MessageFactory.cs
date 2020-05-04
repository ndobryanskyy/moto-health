namespace MotoHealth.Telegram.Messages
{
    public static class MessageFactory
    {
        public static TextMessageBuilder CreateTextMessage()
            => new TextMessageBuilder();
    }
}