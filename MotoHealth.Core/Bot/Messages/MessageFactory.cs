namespace MotoHealth.Core.Bot.Messages
{
    public static class MessageFactory
    {
        public static TextMessageBuilder CreateTextMessage()
            => new TextMessageBuilder();
    }
}