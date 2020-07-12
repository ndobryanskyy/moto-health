using System;
using MotoHealth.Telegram;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.ChatUpdateHandlers
{
    public interface IMainChatUpdateHandlerMessages
    {
        IMessage Start { get; }

        IMessage MotoHealthInfo { get; }
    }

    internal sealed class MainChatUpdateHandlerMessages : IMainChatUpdateHandlerMessages
    {
        public IMessage Start { get; } = MessageFactory.CreateCompositeMessage()
                .AddMessage(StartCommandsHint)
                .AddMessage(StartPinHint);

        public IMessage MotoHealthInfo { get; } = MessageFactory.CreateTextMessage()
            .WithHtml(
                "🚑 <b>MOTO HEALTH</b>\n\n" +
                "<b>Телефон:</b> +380960543434\n" +
                $"<b>Сайт:</b> {TelegramHtml.Link(new Uri("http://www.mh.od.ua"), "mh.od.ua")}\n" +
                $"<b>Instagram:</b> {TelegramHtml.Link(new Uri("https://www.instagram.com/moto_health_odessa"), "@moto_health_odessa")}")
            .WithDisabledWebPagePreview();

        private static readonly IMessage StartCommandsHint = MessageFactory.CreateTextMessage()
            .WithHtml(
                "Нажмите /dtp чтобы получить помощь, если вы стали участником или свидетелем ДТП\n\n" +
                "Эта и другие команды также доступны в меню <b>[ / ]</b> внизу");

        private static readonly IMessage StartPinHint = MessageFactory.CreateTextMessage()
            .WithPlainText("📌 Чтобы не забыть про бота в экстренной ситуации, можете закрепить себе этот диалог");

    }
}