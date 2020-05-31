using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot
{
    public static class CommonMessages
    {
        public static readonly IMessage NotQuiteGetIt = MessageFactory.CreateTextMessage()
            .WithPlainText("🤔 Не совсем понял вас");
    }
}