using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.Commands.AppCommands
{
    internal sealed class StartBotCommand : PrivateChatBotCommandBase
    {
        private static readonly IMessage StartCommandHint = MessageFactory.CreateTextMessage()
            .WithHtml(
                "🚨 Нажмите /dtp чтобы получить помощь, если вы стали участником или свидетелем ДТП\n\n" +
                "👇 Эта и другие команды также доступны в <b>[</b> ☰ <b>]</b> <b>меню</b>");

        private static readonly IMessage StartPinHint = MessageFactory.CreateTextMessage()
            .WithPlainText("📌 Чтобы не забыть про бота в экстренной ситуации, можете закрепить себе этот диалог");

        private static readonly IMessage Start = MessageFactory.CreateCompositeMessage()
            .AddMessage(StartCommandHint)
            .AddMessage(StartPinHint);

        public StartBotCommand()
            : base("/start")
        {
        }

        protected override async Task ExecuteAsync(IChatUpdateContext context, ICommandMessageBotUpdate command, CancellationToken cancellationToken)
            => await context.SendMessageAsync(Start, cancellationToken);
    }
}