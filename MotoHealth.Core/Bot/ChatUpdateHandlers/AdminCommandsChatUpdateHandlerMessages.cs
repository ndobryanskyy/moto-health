using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.ChatUpdateHandlers
{
    public interface IAdminCommandsChatUpdateHandlerMessages
    {
        IMessage ChatSubscribed { get; }

        IMessage ChatUnsubscribed { get; }
    }

    internal sealed class AdminCommandsChatUpdateHandlerMessages : IAdminCommandsChatUpdateHandlerMessages
    {
        public IMessage ChatSubscribed { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("✅ Этот чат будет получать сообщения о ДТП");

        public IMessage ChatUnsubscribed { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("⛔ Этот чат не будет получать сообщения о ДТП");
    }
}