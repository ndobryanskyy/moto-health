using System;
using Microsoft.Azure.Cosmos.Table;

namespace MotoHealth.Functions
{
    public interface ICloudTablesProvider
    {
        CloudTable Accidents { get; }
    }

    internal sealed class CloudTablesProvider : ICloudTablesProvider
    {
        private const string AccidentsTableName = "Accidents";

        private readonly Lazy<CloudTable> _lazyAccidentsTable;

        public CloudTablesProvider(CloudTableClient tablesClient)
        {
            _lazyAccidentsTable = new Lazy<CloudTable>(() => tablesClient.GetTableReference(AccidentsTableName));
        }

        public CloudTable Accidents => _lazyAccidentsTable.Value;
    }
}