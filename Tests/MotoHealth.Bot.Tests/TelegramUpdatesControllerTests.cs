using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using MotoHealth.Bot.Controllers;
using MotoHealth.Bot.Telegram;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types;
using Xunit;

namespace MotoHealth.Bot.Tests
{
    public sealed class TelegramUpdatesControllerTests
    {
        private readonly CancellationToken _cancellationToken = new CancellationToken();

        private readonly Mock<ILogger<TelegramUpdatesController>> _loggerMock;
        private readonly Mock<IBotUpdateResolver> _botUpdateResolverMock;
        private readonly Mock<IChatsFactory> _chatsFactoryMock;

        private readonly TelegramUpdatesController _controller;

        public TelegramUpdatesControllerTests()
        {
            _loggerMock = new Mock<ILogger<TelegramUpdatesController>>();
            _botUpdateResolverMock = new Mock<IBotUpdateResolver>(MockBehavior.Strict);
            _chatsFactoryMock = new Mock<IChatsFactory>(MockBehavior.Strict);

            _controller = new TelegramUpdatesController(
                _loggerMock.Object,
                _botUpdateResolverMock.Object,
                _chatsFactoryMock.Object
            );
        }

        [Fact]
        public async Task Should_Not_Handle_Unsupported_Update()
        {
            IBotUpdate? resolvedUpdate;

            var dummyUpdate = new Update();

            _botUpdateResolverMock
                .Setup(x => x.TryResolveSupportedUpdate(dummyUpdate, out resolvedUpdate))
                .Returns(false);

            await _controller.ReceiveWebHookAsync(dummyUpdate, _cancellationToken);

            _chatsFactoryMock
                .Verify(x => x.CreateChat(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task Should_Handle_Supported_Update()
        {
            const long chatId = 123;

            var telegramChatMock = new Mock<ITelegramChat>(MockBehavior.Strict);
            telegramChatMock.SetupGet(x => x.Id).Returns(chatId);

            var updateMock = new Mock<IBotUpdate>(MockBehavior.Strict);
            updateMock.SetupGet(x => x.Chat).Returns(telegramChatMock.Object);

            IBotUpdate resolvedUpdate = updateMock.Object;

            var dummyUpdate = new Update();

            _botUpdateResolverMock
                .Setup(x => x.TryResolveSupportedUpdate(dummyUpdate, out resolvedUpdate!))
                .Returns(true)
                .Callback(() =>
                {
                    
                });

            var chatMock = new Mock<IChat>(MockBehavior.Strict);
            chatMock
                .Setup(x => x.HandleUpdateAsync(resolvedUpdate, _cancellationToken))
                .Returns(Task.CompletedTask);

            _chatsFactoryMock.Setup(x => x.CreateChat(chatId)).Returns(chatMock.Object);

            await _controller.ReceiveWebHookAsync(dummyUpdate, _cancellationToken);

            chatMock.Verify(x => x.HandleUpdateAsync(resolvedUpdate, _cancellationToken), Times.Once);
        }
    }
}