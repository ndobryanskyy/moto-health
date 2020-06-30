using MotoHealth.Infrastructure.AzureEventGrid;
using MotoHealth.Infrastructure.AzureTables;

namespace MotoHealth.Infrastructure
{
    public sealed class InfrastructureOptions
    {
        public AzureStorageOptions AzureStorage { get; } = new AzureStorageOptions();

        public AzureEventGridOptions AppEventsTopic { get; } = new AzureEventGridOptions();
    }
}