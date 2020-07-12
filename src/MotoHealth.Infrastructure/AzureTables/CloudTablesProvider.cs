using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace MotoHealth.Infrastructure.AzureTables
{
    internal interface ICloudTablesProvider
    {
        CloudTable Chats { get; }

        CloudTable ChatSubscriptions { get; }
    }

    internal sealed class CloudTablesProvider : ICloudTablesProvider
    {
        private const string ChatsTableName = "Chats";
        private const string ChatSubscriptionsTableName = "ChatSubscriptions";

        public CloudTablesProvider(IOptions<InfrastructureOptions> options)
        {
            var azureStorageOptions = options.Value.AzureStorage;

            var storageAccount = CloudStorageAccount.Parse(azureStorageOptions.StorageAccountConnectionString);

            var tablesClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration
            {
                RestExecutorConfiguration = new RestExecutorConfiguration
                {
                    HttpClientTimeout = azureStorageOptions.TablesRequestTimeout
                }
            });

            Chats = tablesClient.GetTableReference(ChatsTableName);
            ChatSubscriptions = tablesClient.GetTableReference(ChatSubscriptionsTableName);
        }

        public CloudTable Chats { get; }

        public CloudTable ChatSubscriptions { get; }
    }
}