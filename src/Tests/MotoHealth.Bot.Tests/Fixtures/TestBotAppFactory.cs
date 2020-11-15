using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Infrastructure.AzureTables;
using MotoHealth.Telegram;

namespace MotoHealth.Bot.Tests.Fixtures
{
    public sealed class TestBotAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            var azureStorageInitializerMock = SetupAzureStorageInitializerMock();

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(azureStorageInitializerMock.Object);
                services.AddSingleton(Mock.Of<ITelegramClient>());
                services.AddSingleton(Mock.Of<IBotInitializer>());

                services.PostConfigure<TelemetryConfiguration>(telemetryOptions =>
                {
                    telemetryOptions.DisableTelemetry = true;
                });
            });
        }

        private static Mock<IAzureStorageInitializer> SetupAzureStorageInitializerMock()
        {
            var initializerMock = new Mock<IAzureStorageInitializer>();

            initializerMock
                .Setup(x => x.InitializeAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return initializerMock;
        }
    }
}