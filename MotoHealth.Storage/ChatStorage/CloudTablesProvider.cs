using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace MotoHealth.Infrastructure.ChatStorage
{
    internal interface ICloudTablesProvider
    {
        CloudTable Chats { get; }
    }

    internal sealed class CloudTablesProvider : ICloudTablesProvider
    {
        private const string ChatsTableName = "Chats";

        public CloudTablesProvider(IOptions<ChatStorageOptions> options)
        {
            var storageAccount = CloudStorageAccount.Parse(options.Value.StorageAccountConnectionString);

            var tablesClient = storageAccount.CreateCloudTableClient();

            Chats = tablesClient.GetTableReference(ChatsTableName);
        }

        public CloudTable Chats { get; }
    }
}