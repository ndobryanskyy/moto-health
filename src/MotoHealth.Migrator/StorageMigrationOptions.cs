namespace MotoHealth.Migrator;

public sealed class StorageMigrationOptions
{
    public string SourceStorageConnectionString { get; set; } = default!;

    public string DestinationStorageConnectionString { get; set; } = default!;
}