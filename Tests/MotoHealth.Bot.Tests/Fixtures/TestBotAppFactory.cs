using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Infrastructure.ChatStorage;
using MotoHealth.Infrastructure.ServiceBus;
using Telegram.Bot;

namespace MotoHealth.Bot.Tests.Fixtures
{
    public sealed class TestBotAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            var serviceBusFactoryMock = SetupServiceBusClientsFactoryMock();
            var azureTablesInitializerMock = SetupAzureTablesInitializerMock();
            var telegramBotClientFactoryMock = SetupTelegramBotClientFactoryMock();

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(serviceBusFactoryMock.Object);
                services.AddSingleton(azureTablesInitializerMock.Object);
                services.AddSingleton(telegramBotClientFactoryMock.Object);
            });
        }

        private static Mock<IServiceBusClientsFactory> SetupServiceBusClientsFactoryMock()
        {
            var factoryMock = new Mock<IServiceBusClientsFactory>();

            factoryMock
                .Setup(x => x.CreateSessionHandlingClient(It.IsAny<ServiceBusConnectionStringBuilder>()))
                .Returns(Mock.Of<IQueueClient>());

            factoryMock
                .Setup(x => x.CreateMessageSenderClient(It.IsAny<ServiceBusConnectionStringBuilder>()))
                .Returns(Mock.Of<IMessageSender>());

            return factoryMock;
        }

        private static Mock<IAzureTablesInitializer> SetupAzureTablesInitializerMock()
        {
            var initializerMock = new Mock<IAzureTablesInitializer>();

            initializerMock
                .Setup(x => x.InitializeAllAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return initializerMock;
        }

        private static Mock<ITelegramBotClientFactory> SetupTelegramBotClientFactoryMock()
        {
            var factoryMock = new Mock<ITelegramBotClientFactory>();

            factoryMock
                .Setup(x => x.CreateClient())
                .Returns(Mock.Of<ITelegramBotClient>());

            return factoryMock;
        }
    }
}