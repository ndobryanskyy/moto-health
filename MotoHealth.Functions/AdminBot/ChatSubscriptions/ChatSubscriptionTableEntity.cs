using Microsoft.Azure.Cosmos.Table;

namespace MotoHealth.Functions.AdminBot.ChatSubscriptions
{
    public sealed class ChatSubscriptionTableEntity : TableEntity, IChatSubscription
    {
        private long _chatId;
        private string _topic = string.Empty;

        public long ChatId
        {
            get => _chatId;
            set
            {
                _chatId = value;
                RowKey = value.ToString();
            }
        }

        public string Topic
        {
            get => _topic;
            set => _topic = PartitionKey = value;
        }

        public bool IsEnabled { get; set; }
    }
}