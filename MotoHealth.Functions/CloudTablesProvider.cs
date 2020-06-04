using System;
using Microsoft.Azure.Cosmos.Table;

namespace MotoHealth.Functions
{
    public interface ICloudTablesProvider
    {
        CloudTable ChatSubscriptionsEvents { get; }

        CloudTable Accidents { get; }
    }

    internal sealed class CloudTablesProvider : ICloudTablesProvider
    {
        private const string ChatSubscriptionsEventsTableName = "ChatSubscriptionsEvents";
        private const string AccidentsTableName = "Accidents";

        private readonly Lazy<CloudTable> _lazyChatSubscriptionsTable;
        private readonly Lazy<CloudTable> _lazyAccidentsTable;

        public CloudTablesProvider(CloudTableClient tablesClient)
        {
            _lazyChatSubscriptionsTable = new Lazy<CloudTable>(() => tablesClient.GetTableReference(ChatSubscriptionsEventsTableName));
            _lazyAccidentsTable = new Lazy<CloudTable>(() => tablesClient.GetTableReference(AccidentsTableName));
        }

        public CloudTable ChatSubscriptionsEvents => _lazyChatSubscriptionsTable.Value;

        public CloudTable Accidents => _lazyAccidentsTable.Value;
    }
}