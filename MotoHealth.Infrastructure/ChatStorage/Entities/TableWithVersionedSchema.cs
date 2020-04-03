using Microsoft.Azure.Cosmos.Table;

namespace MotoHealth.Infrastructure.ChatStorage.Entities
{
    internal abstract class TableWithVersionedSchema : TableEntity
    {
        [IgnoreProperty]
        public abstract int LatestEntitySchemaVersion { get; }

        public int EntitySchemaVersion { get; set; } = 1;
    }
}