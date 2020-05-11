using System;
using Microsoft.Azure.Cosmos.Table;

namespace MotoHealth.Functions
{
    public interface ICloudTablesProvider
    {
        CloudTable ChatSubscriptions { get; }

        CloudTable Accidents { get; }
    }

    internal sealed class CloudTablesProvider : ICloudTablesProvider
    {
        private const string ChatSubscriptionsTableName = "ChatSubscriptions";
        private const string AccidentsTableName = "Accidents";

        private readonly Lazy<CloudTable> _lazyChatSubscriptionsTable;
        private readonly Lazy<CloudTable> _lazyAccidentsTable;

        public CloudTablesProvider(CloudTableClient tablesClient)
        {
            _lazyChatSubscriptionsTable = new Lazy<CloudTable>(() => tablesClient.GetTableReference(ChatSubscriptionsTableName));
            _lazyAccidentsTable = new Lazy<CloudTable>(() => tablesClient.GetTableReference(AccidentsTableName));
        }

        public CloudTable ChatSubscriptions => _lazyChatSubscriptionsTable.Value;

        public CloudTable Accidents => _lazyAccidentsTable.Value;
    }
}