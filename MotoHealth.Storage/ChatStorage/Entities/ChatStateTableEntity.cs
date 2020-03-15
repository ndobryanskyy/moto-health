using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Infrastructure.ChatStorage.Entities
{
    internal sealed class ChatStateTableEntity : TableEntity, IChatState
    {
        /// <summary>
        /// Used for deserialization
        /// </summary>
        public ChatStateTableEntity()
        {
            RowKey = ChatsTableEntityTypes.State;
        }

        public ChatStateTableEntity(long chatId) : this()
        {
            AssociatedChatId = chatId;
            PartitionKey = chatId.ToString();
        }

        public long AssociatedChatId { get; set; }

        public bool UserSubscribed { get; set; }
    }
}