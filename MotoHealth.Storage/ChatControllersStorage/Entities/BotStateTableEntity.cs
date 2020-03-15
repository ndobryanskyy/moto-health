using Microsoft.Azure.Cosmos.Table;

namespace MotoHealth.Infrastructure.ChatControllersStorage.Entities
{
    public sealed class BotStateTableEntity : TableEntity
    {
        private long _chatId;

        public BotStateTableEntity()
        {
            RowKey = BotsTableEntityTypes.State;
        }

        public long ChatId
        {
            get => _chatId;
            set
            {
                _chatId = value;

                PartitionKey = _chatId.ToString();
            }
        }

        public bool IsUserSubscribed { get; set; }
    }
}