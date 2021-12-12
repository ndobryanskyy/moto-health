using MotoHealth.Migrator;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddOptions<StorageMigrationOptions>()
            .BindConfiguration(nameof(StorageMigrationOptions));

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
