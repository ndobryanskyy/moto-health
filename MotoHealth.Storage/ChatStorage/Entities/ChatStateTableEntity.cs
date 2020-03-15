using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Infrastructure.ChatStorage.Entities
{
    public sealed class ChatStateTableEntity : TableEntity, IChatState
    {
        private long _associatedChatId;

        public ChatStateTableEntity(long chatId)
        {
            AssociatedChatId = chatId;

            RowKey = ChatsTableEntityTypes.State;
        }

        public long AssociatedChatId
        {
            get => _associatedChatId;
            private set
            {
                _associatedChatId = value;

                PartitionKey = _associatedChatId.ToString();
            }
        }

        public bool UserSubscribed { get; set; }
    }
}