﻿using Microsoft.Azure.Cosmos.Table;

namespace MotoHealth.Functions.ChatSubscriptions
{
    public sealed class ChatSubscriptionTableEntity : TableEntity
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

        public string ChatName { get; set; } = string.Empty;

        public string Topic
        {
            get => _topic;
            set => _topic = PartitionKey = value;
        }

        public bool IsEnabled { get; set; }
    }
}