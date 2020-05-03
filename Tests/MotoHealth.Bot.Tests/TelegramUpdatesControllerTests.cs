using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using MotoHealth.Bot.Controllers;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Core.Telegram;
using Telegram.Bot.Types;
using Xunit;

namespace MotoHealth.Bot.Tests
{
    public sealed class TelegramUpdatesControllerTests
    {
        private readonly CancellationToken _cancellationToken = new CancellationToken();

        private readonly Mock<ILogger<TelegramUpdatesController>> _loggerMock;
        private readonly Mock<IBotTelemetryService> _botTelemetryMock;
        private readonly Mock<IBotUpdatesMapper> _botUpdatesMapperMock;
        private readonly Mock<IChatsFactory> _chatsFactoryMock;

        private readonly TelegramUpdatesController _controller;

        public TelegramUpdatesControllerTests()
        {
            _loggerMock = new Mock<ILogger<TelegramUpdatesController>>();
            _botTelemetryMock = new Mock<IBotTelemetryService>(MockBehavior.Strict);
            _botUpdatesMapperMock = new Mock<IBotUpdatesMapper>(MockBehavior.Strict);
            _chatsFactoryMock = new Mock<IChatsFactory>(MockBehavior.Strict);

            _controller = new TelegramUpdatesController(
                _loggerMock.Object,
                _botUpdatesMapperMock.Object,
                _chatsFactoryMock.Object,
                _botTelemetryMock.Object
            );
        }

        [Fact]
        public async Task Should_Not_Handle_Non_Chat_Update()
        {
            var dummyUpdate = new Update();

            var mappedNonChatUpdate = Mock.Of<INotMappedBotUpdate>();

            _botUpdatesMapperMock
                .Setup(x => x.MapTelegramUpdate(dummyUpdate))
                .Returns(mappedNonChatUpdate);

            _botTelemetryMock.Setup(x => x.OnUpdateMapped(mappedNonChatUpdate));
            _botTelemetryMock.Setup(x => x.OnUpdateSkipped());

            await _controller.ReceiveWebHookAsync(dummyUpdate, _cancellationToken);

            _chatsFactoryMock
                .Verify(x => x.CreateChat(It.IsAny<long>()), Times.Never);

            _botTelemetryMock.Verify(x => x.OnUpdateMapped(mappedNonChatUpdate), Times.Once);
            _botTelemetryMock.Verify(x => x.OnUpdateSkipped(), Times.Once);
        }

        [Fact]
        public async Task Should_Handle_Supported_Update()
        {
            const long chatId = 123;

            var telegramChatMock = new Mock<ITelegramChat>(MockBehavior.Strict);
            telegramChatMock.SetupGet(x => x.Id).Returns(chatId);

            var mappedChatUpdateMock = new Mock<IChatUpdate>(MockBehavior.Strict);
            mappedChatUpdateMock.SetupGet(x => x.Chat).Returns(telegramChatMock.Object);

            _botTelemetryMock.Setup(x => x.OnUpdateMapped(mappedChatUpdateMock.Object));

            var dummyUpdate = new Update();

            _botUpdatesMapperMock
                .Setup(x => x.MapTelegramUpdate(dummyUpdate))
                .Returns(mappedChatUpdateMock.Object);

            var chatMock = new Mock<IChat>(MockBehavior.Strict);
            chatMock
                .Setup(x => x.HandleUpdateAsync(mappedChatUpdateMock.Object, _cancellationToken))
                .Returns(Task.CompletedTask);

            _chatsFactoryMock.Setup(x => x.CreateChat(chatId)).Returns(chatMock.Object);

            await _controller.ReceiveWebHookAsync(dummyUpdate, _cancellationToken);

            _botTelemetryMock.Verify(x => x.OnUpdateMapped(mappedChatUpdateMock.Object), Times.Once);
            chatMock.Verify(x => x.HandleUpdateAsync(mappedChatUpdateMock.Object, _cancellationToken), Times.Once);
        }
    }
}