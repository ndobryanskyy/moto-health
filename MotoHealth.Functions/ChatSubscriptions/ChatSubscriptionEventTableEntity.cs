using System;
using Microsoft.Azure.Cosmos.Table;

namespace MotoHealth.Functions.ChatSubscriptions
{
    public sealed class ChatSubscriptionEventTableEntity : TableEntity
    {
        [IgnoreProperty]
        public string EventId
        {
            get => RowKey;
            set => RowKey = value;
        }

        [IgnoreProperty]
        public string Topic
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        public long ChatId { get; set; }

        public bool IsSubscribed { get; set; }

        public DateTime EventTime { get; set; }
    }
}