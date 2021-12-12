using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace MotoHealth.Migrator;

public class Worker : BackgroundService
{
    private const int DegreeOfParallelism = 5;

    private readonly ILogger<Worker> _logger;
    private readonly ILoggerFactory _loggerFactory;

    private readonly CloudTableClient _sourceTablesClient;
    private readonly CloudTableClient _destinationTablesClient;

    public Worker(
        ILogger<Worker> logger,
        ILoggerFactory loggerFactory,
        IOptions<StorageMigrationOptions> storageOptions)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;

        _sourceTablesClient = CloudStorageAccount.Parse(storageOptions.Value.SourceStorageConnectionString).CreateCloudTableClient();
        _destinationTablesClient = CloudStorageAccount.Parse(storageOptions.Value.DestinationStorageConnectionString).CreateCloudTableClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await CopyAllRowsAsync("ChatSubscriptions", stoppingToken);
        await CopyAllRowsAsync("Chats", stoppingToken);
        await CopyAllRowsAsync("Accidents", stoppingToken);

        _logger.LogInformation("Everything completed!");
    }

    private async Task CopyAllRowsAsync(string tableName, CancellationToken cancellationToken)
    {
        var sourceTable = _sourceTablesClient.GetTableReference(tableName);
        var destinationTable = _destinationTablesClient.GetTableReference(tableName);

        var pipelineLogger = _loggerFactory.CreateLogger($"TablePipeline:{tableName}");

        await destinationTable.CreateIfNotExistsAsync(cancellationToken);

        pipelineLogger.LogInformation("Destination table created");

        var sourceEntitiesBuffer = new BufferBlock<DynamicTableEntity>(new DataflowBlockOptions
        {
            EnsureOrdered = false
        });

        var insertActionBlock = new ActionBlock<DynamicTableEntity>(async (sourceEntity) =>
            {
                var destinationOperation = TableOperation.InsertOrReplace(sourceEntity);
                await destinationTable.ExecuteAsync(destinationOperation, cancellationToken);
            },
            new ExecutionDataflowBlockOptions
            {
                EnsureOrdered = false,
                MaxDegreeOfParallelism = DegreeOfParallelism,
                BoundedCapacity = DegreeOfParallelism,
                CancellationToken = cancellationToken
            });

        sourceEntitiesBuffer.LinkTo(insertActionBlock, new DataflowLinkOptions { PropagateCompletion = true });

        pipelineLogger.LogInformation("Started copying...");
        var stopwatch = Stopwatch.StartNew();

        await foreach (var sourceItem in GetAllRowsAsync(sourceTable, cancellationToken))
        {
            await sourceEntitiesBuffer.SendAsync(sourceItem, cancellationToken);
        }

        sourceEntitiesBuffer.Complete();

        await sourceEntitiesBuffer.Completion;

        stopwatch.Stop();
        pipelineLogger.LogInformation($"Completed copying. Elapsed: {stopwatch.Elapsed}");
    }

    private async IAsyncEnumerable<DynamicTableEntity> GetAllRowsAsync(
        CloudTable table,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var query = new TableQuery();
        TableContinuationToken? continuationToken = null;

        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            var segment = await table.ExecuteQuerySegmentedAsync(query, continuationToken, cancellationToken);

            foreach (var item in segment)
            {
                yield return item;
            }

            continuationToken = segment.ContinuationToken;
        } while (continuationToken != null);
    }
}