using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MotoHealth.Infrastructure.ChatStorage;
using MotoHealth.Infrastructure.ServiceBus;

namespace MotoHealth.Bot.Tests.Fixtures
{
    public sealed class TestBotAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            var serviceBusFactoryMock = SetupServiceBusClientsFactoryMock();
            var azureTablesInitializerMock = SetupAzureTablesInitializerMock();

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(serviceBusFactoryMock.Object);
                services.AddSingleton(azureTablesInitializerMock.Object);
            });
        }

        private static Mock<IServiceBusClientsFactory> SetupServiceBusClientsFactoryMock()
        {
            var serviceBusFactoryMock = new Mock<IServiceBusClientsFactory>();

            serviceBusFactoryMock
                .Setup(x => x.CreateSessionHandlingClient(It.IsAny<ServiceBusConnectionStringBuilder>()))
                .Returns(Mock.Of<IQueueClient>());

            serviceBusFactoryMock
                .Setup(x => x.CreateMessageSenderClient(It.IsAny<ServiceBusConnectionStringBuilder>()))
                .Returns(Mock.Of<IMessageSender>());

            return serviceBusFactoryMock;
        }

        private static Mock<IAzureTablesInitializer> SetupAzureTablesInitializerMock()
        {
            var initializerMock = new Mock<IAzureTablesInitializer>();

            initializerMock
                .Setup(x => x.InitializeAllAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return initializerMock;
        }
    }
}