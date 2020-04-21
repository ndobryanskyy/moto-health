using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using MotoHealth.Bot.Telegram;
using MotoHealth.Bot.Tests.Fixtures.Telegram;
using MotoHealth.Core.Bot.Updates;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;

namespace MotoHealth.Bot.Tests
{
    public sealed class BotUpdateResolverTests
    {
        private readonly Mock<IMapper> _mapperMock;

        private readonly BotUpdateResolver _resolver;

        public BotUpdateResolverTests()
        {
            _mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            _resolver = new BotUpdateResolver(_mapperMock.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Should_Resolve_TextMessage_Update(bool messageInGroup)
        {
            var autoFixture = new Fixture();

            var messageBuilder = messageInGroup
                ? autoFixture.BuildDefaultGroupMessage()
                : autoFixture.BuildDefaultPrivateMessage();
            
            var message = messageBuilder
                .With(x => x.Text, "Test message.")
                .Create();

            var update = autoFixture.BuildDefaultUpdate()
                .With(x => x.Message, message)
                .Create();

            var mappedTextMessageBotUpdate = new TextMessageBotUpdate();

            _mapperMock
                .Setup(x => x.Map<TextMessageBotUpdate>(update))
                .Returns(mappedTextMessageBotUpdate);

            var updateResolved = _resolver.TryResolveSupportedUpdate(update, out var supportedUpdate);

            updateResolved.Should().BeTrue();

            supportedUpdate.Should().BeSameAs(mappedTextMessageBotUpdate);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Should_Resolve_Command_Update(bool messageInGroup)
        {
            const string sampleCommand = "/start";

            var autoFixture = new Fixture();

            var messageEntities = new[]
            {
                new MessageEntity
                {
                    Type = MessageEntityType.BotCommand,
                    Offset = 0,
                    Length = sampleCommand.Length
                }
            };

            var messageBuilder = messageInGroup
                ? autoFixture.BuildDefaultGroupMessage()
                : autoFixture.BuildDefaultPrivateMessage();

            var message = messageBuilder
                .With(x => x.Text, sampleCommand)
                .With(x => x.Entities, messageEntities)
                .Create();

            var update = autoFixture.BuildDefaultUpdate()
                .With(x => x.Message, message)
                .Create();

            var mappedCommandBotUpdate = new CommandMessageBotUpdate();

            _mapperMock
                .Setup(x => x.Map<CommandMessageBotUpdate>(update))
                .Returns(mappedCommandBotUpdate);

            var updateResolved = _resolver.TryResolveSupportedUpdate(update, out var supportedUpdate);

            updateResolved.Should().BeTrue();

            supportedUpdate.Should().BeSameAs(mappedCommandBotUpdate);
        }

        [Fact]
        public void Should_Resolve_TextMessage_Update_If_More_Than_One_MessageEntity_Present()
        {
            var autoFixture = new Fixture();

            const string command = "/start";
            const string url = "https://sample-url.org";

            var text = $"{command} {url}";

            var messageEntities = new[]
            {
                new MessageEntity
                {
                    Type = MessageEntityType.BotCommand,
                    Offset = 0,
                    Length = command.Length
                },

                new MessageEntity
                {
                    Type = MessageEntityType.Url,
                    Offset = command.Length + 1,
                    Length = url.Length
                }
            };

            var message = autoFixture
                .BuildDefaultPrivateMessage()
                .With(x => x.Text, text)
                .With(x => x.Entities, messageEntities)
                .Create();

            var update = autoFixture.BuildDefaultUpdate()
                .With(x => x.Message, message)
                .Create();

            var mappedTextMessageBotUpdate = new TextMessageBotUpdate();

            _mapperMock
                .Setup(x => x.Map<TextMessageBotUpdate>(update))
                .Returns(mappedTextMessageBotUpdate);

            var updateResolved = _resolver.TryResolveSupportedUpdate(update, out var supportedUpdate);

            updateResolved.Should().BeTrue();

            supportedUpdate.Should().BeSameAs(mappedTextMessageBotUpdate);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Should_Resolve_Contact_Update(bool messageInGroup)
        {
            var autoFixture = new Fixture();

            var messageBuilder = messageInGroup
                ? autoFixture.BuildDefaultGroupMessage()
                : autoFixture.BuildDefaultPrivateMessage();

            var message = messageBuilder
                .With(x => x.Contact)
                .Create();

            var update = autoFixture.BuildDefaultUpdate()
                .With(x => x.Message, message)
                .Create();

            var mappedContactUpdate = new ContactMessageBotUpdate();

            _mapperMock
                .Setup(x => x.Map<ContactMessageBotUpdate>(update))
                .Returns(mappedContactUpdate);

            var updateResolved = _resolver.TryResolveSupportedUpdate(update, out var supportedUpdate);

            updateResolved.Should().BeTrue();

            supportedUpdate.Should().BeSameAs(mappedContactUpdate);
        }
    }
}