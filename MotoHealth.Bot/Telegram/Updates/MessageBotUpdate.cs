using Telegram.Bot.Types;

namespace MotoHealth.Bot.Telegram.Updates
{
    internal abstract class MessageBotUpdate : BotUpdateBase, IMessageBotUpdate
    {
        protected MessageBotUpdate(Update update)
            : base(update)
        {
        }

        public Message Message => OriginalUpdate.Message;

        public override Chat Chat => Message.Chat;
    }
}