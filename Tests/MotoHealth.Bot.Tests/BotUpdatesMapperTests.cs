using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using MotoHealth.Bot.Tests.Fixtures.Telegram;
using MotoHealth.Core.Bot.Updates;
using MotoHealth.Core.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;

namespace MotoHealth.Bot.Tests
{
    public sealed class BotUpdatesMapperTests
    {
        private readonly Mock<IMapper> _mapperMock;

        private readonly BotUpdatesMapper _updatesMapper;

        public BotUpdatesMapperTests()
        {
            _mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            _updatesMapper = new BotUpdatesMapper(_mapperMock.Object);
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

            var expectedMappedUpdate = new TextMessageBotUpdate();

            _mapperMock
                .Setup(x => x.Map<TextMessageBotUpdate>(update))
                .Returns(expectedMappedUpdate);

            var mappedUpdate = _updatesMapper.MapTelegramUpdate(update);

            mappedUpdate.Should().BeSameAs(expectedMappedUpdate);
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

            var expectedMappedUpdate = new CommandMessageBotUpdate();

            _mapperMock
                .Setup(x => x.Map<CommandMessageBotUpdate>(update))
                .Returns(expectedMappedUpdate);

            var mappedUpdate = _updatesMapper.MapTelegramUpdate(update);

            mappedUpdate.Should().BeSameAs(expectedMappedUpdate);
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

            var expectedMappedUpdate = new TextMessageBotUpdate();

            _mapperMock
                .Setup(x => x.Map<TextMessageBotUpdate>(update))
                .Returns(expectedMappedUpdate);

            var mappedUpdate = _updatesMapper.MapTelegramUpdate(update);

            mappedUpdate.Should().BeSameAs(expectedMappedUpdate);
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

            var expectedMappedUpdate = new ContactMessageBotUpdate();

            _mapperMock
                .Setup(x => x.Map<ContactMessageBotUpdate>(update))
                .Returns(expectedMappedUpdate);

            var mappedUpdate = _updatesMapper.MapTelegramUpdate(update);

            mappedUpdate.Should().BeSameAs(expectedMappedUpdate);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Should_Resolve_Location_Update(bool messageInGroup)
        {
            var autoFixture = new Fixture();

            var messageBuilder = messageInGroup
                ? autoFixture.BuildDefaultGroupMessage()
                : autoFixture.BuildDefaultPrivateMessage();

            var message = messageBuilder
                .With(x => x.Location)
                .Create();

            var update = autoFixture.BuildDefaultUpdate()
                .With(x => x.Message, message)
                .Create();

            var expectedMappedUpdate = new LocationMessageBotUpdate();

            _mapperMock
                .Setup(x => x.Map<LocationMessageBotUpdate>(update))
                .Returns(expectedMappedUpdate);

            var mappedUpdate = _updatesMapper.MapTelegramUpdate(update);

            mappedUpdate.Should().BeSameAs(expectedMappedUpdate);
        }

        [Fact]
        public void Should_Return_Update_Of_NotMapped_Type_If_No_Mapping_Exists()
        {
            var autoFixture = new Fixture();

            var update = autoFixture.BuildDefaultUpdate()
                .With(x => x.PreCheckoutQuery)
                .Create();

            var expectedMappedUpdate = new NotMappedBotUpdate();

            _mapperMock
                .Setup(x => x.Map<NotMappedBotUpdate>(update))
                .Returns(expectedMappedUpdate);

            var mappedUpdate = _updatesMapper.MapTelegramUpdate(update);

            mappedUpdate.Should().BeSameAs(expectedMappedUpdate);
        }

        [Fact]
        public void Should_Return_Update_Of_NotMappedMessage_Type_If_No_Mapping_For_Message_Type_Exists()
        {
            var autoFixture = new Fixture();

            var message = autoFixture.BuildDefaultPrivateMessage()
                .With(x => x.Audio)
                .Create();

            var update = autoFixture.BuildDefaultUpdate()
                .With(x => x.Message, message)
                .Create();

            var expectedMappedUpdate = new NotMappedMessageBotUpdate();

            _mapperMock
                .Setup(x => x.Map<NotMappedMessageBotUpdate>(update))
                .Returns(expectedMappedUpdate);

            var mappedUpdate = _updatesMapper.MapTelegramUpdate(update);

            mappedUpdate.Should().BeSameAs(expectedMappedUpdate);
        }
    }
}