using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot
{
    public interface IMainChatMessages
    {
        IMessage Start { get; }

        IMessage MotoHealthInfo { get; }

        IMessage NothingToSay { get; }

        IMessage PleaseTryLater { get; }
    }

    internal sealed class MainChatMessages : IMainChatMessages
    {
        public IMessage Start { get; } = MessageFactory.CreateCompositeMessage()
                .AddMessage(StartCommandsHint)
                .AddMessage(StartPinHint);

        public IMessage MotoHealthInfo { get; } = MessageFactory.CreateTextMessage()
            .WithHtml(
                "🚑 <b>MOTO HEALTH</b>\n\n" +
                "<b>Телефон:</b> +380960543434\n" +
                @"<b>Сайт:</b> <a href=""http://www.mh.od.ua"">mh.od.ua</a>" + "\n" +
                @"<b>Instagram:</b> <a href=""https://www.instagram.com/moto_health_odessa"">@moto_health_odessa</a>")
            .WithDisabledWebPagePreview();

        public IMessage NothingToSay { get; } = MessageFactory.CreateCompositeMessage()
            .AddMessage(CommonMessages.NotQuiteGetIt)
            .AddMessage(NothingToSayHint);

        public IMessage PleaseTryLater { get; } = MessageFactory.CreateTextMessage()
            .WithPlainText(
                "😥 Ой, что-то пошло не так\n\n" + 
                "Попробуйте ещё раз через пару секунд");

        private static readonly IMessage StartCommandsHint = MessageFactory.CreateTextMessage()
            .WithHtml(
                "Нажмите /dtp чтобы получить помощь, если вы стали участником или свидетелем ДТП\n\n" +
                "Эта и другие команды также доступны в меню <b>[ / ]</b> внизу");

        private static readonly IMessage StartPinHint = MessageFactory.CreateTextMessage()
            .WithPlainText("📌 Чтобы не забыть про бота в экстренной ситуации, можете закрепить себе этот диалог");

        private static readonly IMessage NothingToSayHint = MessageFactory.CreateTextMessage()
            .WithHtml("Пожалуйста, выберите команду в меню <b>[ / ]</b> внизу");
    }
}