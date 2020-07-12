using System.Globalization;
using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Infrastructure.ChatSubscriptions
{
    internal sealed class ChatSubscriptionTableEntity : TableEntity, IChatSubscription
    {
        private long _chatId;

        [IgnoreProperty]
        public string Topic
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        public long ChatId
        {
            get => _chatId;
            set
            {
                _chatId = value;

                RowKey = value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}