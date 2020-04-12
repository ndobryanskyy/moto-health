namespace MotoHealth.Core.Bot.Messages
{
    internal interface IMessageFactory
    {
        TextMessageBuilder CreateTextMessage();
    }

    internal sealed class MessageFactory : IMessageFactory
    {
        public TextMessageBuilder CreateTextMessage()
            => new TextMessageBuilder();
    }
}