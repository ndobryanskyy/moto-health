namespace MotoHealth.Core.Bot.Messages
{
    internal interface IMessageFactory
    {
        TextMessageBuilder CreateTextMessage(string text);
    }

    internal sealed class MessageFactory : IMessageFactory
    {
        public TextMessageBuilder CreateTextMessage(string text)
            => new TextMessageBuilder(text);
    }
}