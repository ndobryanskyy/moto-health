using System.Runtime.InteropServices.ComTypes;
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
        private readonly Mock<IBotUpdatesQueue> _botUpdatesQueueMock;
        private readonly TelegramUpdatesController _controller;

        public TelegramUpdatesControllerTests()
        {
            _loggerMock = new Mock<ILogger<TelegramUpdatesController>>();
            _botUpdateResolverMock = new Mock<IBotUpdateResolver>();
            _botUpdatesQueueMock = new Mock<IBotUpdatesQueue>();

            _controller = new TelegramUpdatesController(
                _loggerMock.Object,
                _botUpdateResolverMock.Object,
                _botUpdatesQueueMock.Object
            );
        }

        [Fact]
        public async Task Should_Not_Add_Unsupported_Update_To_Queue()
        {
            IBotUpdate? resolvedUpdate;

            var dummyUpdate = new Update();

            _botUpdateResolverMock
                .Setup(x => x.TryResolveSupportedUpdate(dummyUpdate, out resolvedUpdate))
                .Returns(false);

            await _controller.ReceiveWebHookAsync(dummyUpdate, _cancellationToken);

            _botUpdatesQueueMock
                .Verify(x => x.EnqueueUpdateAsync(It.IsAny<IBotUpdate>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_Add_Supported_Update_To_Queue()
        {
            IBotUpdate? resolvedUpdate = null;

            var dummyUpdate = new Update();

            _botUpdateResolverMock
                .Setup(x => x.TryResolveSupportedUpdate(dummyUpdate, out resolvedUpdate))
                .Returns(true);

            await _controller.ReceiveWebHookAsync(dummyUpdate, _cancellationToken);

            _botUpdatesQueueMock
                .Verify(x => x.EnqueueUpdateAsync(resolvedUpdate!, _cancellationToken), Times.Once);
        }
    }
}