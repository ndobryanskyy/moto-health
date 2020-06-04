using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot
{
    public interface IAdminHandlerMessages
    {
        IMessage ChatSubscribed { get; }

        IMessage ChatUnsubscribed { get; }
    }

    internal sealed class AdminHandlerMessages : IAdminHandlerMessages
    {
        public IMessage ChatSubscribed { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("✅ Этот чат будет получать сообщения о ДТП");

        public IMessage ChatUnsubscribed { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("⛔ Этот чат не будет получать сообщения о ДТП");
    }
}