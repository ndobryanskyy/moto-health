using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Infrastructure.ChatStorage.Entities
{
    internal sealed class ChatStateTableEntity : TableWithVersionedSchema, IChatState
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
            EntitySchemaVersion = LatestEntitySchemaVersion;

            AssociatedChatId = chatId;
            PartitionKey = chatId.ToString();
        }

        [IgnoreProperty]
        public override int LatestEntitySchemaVersion => 1;

        public long AssociatedChatId { get; set; }

        public bool UserSubscribed { get; set; }
    }
}