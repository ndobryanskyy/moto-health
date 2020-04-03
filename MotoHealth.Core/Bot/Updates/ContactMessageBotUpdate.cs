using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public sealed class ContactMessageBotUpdate : MessageBotUpdateBase, IContactMessageBotUpdate
    {
        public ContactMessageBotUpdate(
            int updateId, 
            int messageId, 
            TelegramChat chat,
            TelegramContact contact) 
            : base(updateId, messageId, chat)
        {
            Contact = contact;
        }

        public ITelegramContact Contact { get; }
    }
}