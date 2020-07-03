using System;

namespace MotoHealth.Infrastructure.AzureTables
{
    public sealed class AzureStorageOptions
    {
        public string StorageAccountConnectionString { get; set; } = default!;

        public int TablesRequestTimeoutInSeconds { get; set; } = 20;

        public TimeSpan TablesRequestTimeout => TimeSpan.FromSeconds(TablesRequestTimeoutInSeconds);

        public int QueuesRequestTimeoutInSeconds { get; set; } = 60;

        public TimeSpan QueuesRequestTimeout => TimeSpan.FromSeconds(QueuesRequestTimeoutInSeconds);
    }
}