using MotoHealth.Telegram;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.ChatUpdateHandlers
{
    public interface IAdminCommandsChatUpdateHandlerMessages
    {
        IMessage ChatWasSubscribed { get; }

        IMessage ChatWasUnsubscribed { get; }

        IMessage CannotBanOrUnbanSelf { get; }

        IMessage CannotBanOrUnbanUnknownUser { get; }

        IMessage UserIsAlreadyBanned(int userId);

        IMessage UserIsNotBanned(int userId);

        IMessage UserWasBanned(int userId);

        IMessage UserWasUnbanned(int userId);
    }

    internal sealed class AdminCommandsChatUpdateHandlerMessages : IAdminCommandsChatUpdateHandlerMessages
    {
        public IMessage ChatWasSubscribed { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("✅ Этот чат будет получать сообщения о ДТП");

        public IMessage ChatWasUnsubscribed { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("⛔ Этот чат не будет получать сообщения о ДТП");

        public IMessage CannotBanOrUnbanSelf { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("⚠️ Нельзя забанить или разбанить себя");

        public IMessage CannotBanOrUnbanUnknownUser { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText("⚠️ Нельзя забанить или разбанить пользователя, которого бот не знает");

        public IMessage UserIsAlreadyBanned(int userId) => MessageFactory.CreateTextMessage()
            .WithHtml($"⚠️ Пользователь {TelegramHtml.UserLink(userId, userId.ToString())} уже забанен");

        public IMessage UserIsNotBanned(int userId) => MessageFactory.CreateTextMessage()
            .WithHtml($"⚠️ Пользователь {TelegramHtml.UserLink(userId, userId.ToString())} уже разбанен");

        public IMessage UserWasBanned(int userId) => MessageFactory.CreateTextMessage()
            .WithHtml($"⛔ Пользователь {TelegramHtml.UserLink(userId, userId.ToString())} забанен");

        public IMessage UserWasUnbanned(int userId) => MessageFactory.CreateTextMessage()
            .WithHtml($"✅ Пользователь {TelegramHtml.UserLink(userId, userId.ToString())} разбанен");
    }
}