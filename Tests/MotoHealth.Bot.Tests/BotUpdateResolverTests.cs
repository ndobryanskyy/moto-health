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

        public BotUpdateResolverTests()
        {
            _mapperMock = new Mock<IMapper>(MockBehavior.Strict);
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

            var mappedUser = new TelegramUser();

            _mapperMock
                .Setup(x => x.Map<TelegramUser>(message.From))
                .Returns(mappedUser);

            var mappedGroup = new TelegramGroup();

            _mapperMock
                .Setup(x => x.Map<TelegramGroup>(message.Chat))
                .Returns(mappedGroup);

            var update = autoFixture.BuildDefaultUpdate()
                .With(x => x.Message, message)
                .Create();

            var resolver = CreateResolver();

            var updateResolved = resolver.TryResolveSupportedUpdate(update, out var supportedUpdate);

            updateResolved.Should().BeTrue();

            var textMessageUpdate = supportedUpdate.Should().BeOfType<TextMessageBotUpdate>().Subject;

            textMessageUpdate.UpdateId.Should().Be(update.Id);
            textMessageUpdate.MessageId.Should().Be(message.MessageId);

            textMessageUpdate.Text.Should().Be(message.Text);

            textMessageUpdate.Chat.Id.Should().Be(message.Chat.Id);
            textMessageUpdate.Chat.From.Should().BeSameAs(mappedUser);

            if (messageInGroup)
            {
                textMessageUpdate.Chat.Group.Should().BeSameAs(mappedGroup);
            }
            else
            {
                textMessageUpdate.Chat.Group.Should().BeNull();
            }
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
                .With(x => x.Text, $"{sampleCommand}    a       b")
                .With(x => x.Entities, messageEntities)
                .Create();

            var mappedUser = new TelegramUser();

            _mapperMock
                .Setup(x => x.Map<TelegramUser>(message.From))
                .Returns(mappedUser);

            var mappedGroup = new TelegramGroup();

            _mapperMock
                .Setup(x => x.Map<TelegramGroup>(message.Chat))
                .Returns(mappedGroup);

            var update = autoFixture.BuildDefaultUpdate()
                .With(x => x.Message, message)
                .Create();

            var resolver = CreateResolver();

            var updateResolved = resolver.TryResolveSupportedUpdate(update, out var supportedUpdate);

            updateResolved.Should().BeTrue();

            var commandUpdate = supportedUpdate.Should().BeOfType<CommandBotUpdate>().Subject;

            commandUpdate.UpdateId.Should().Be(update.Id);
            commandUpdate.MessageId.Should().Be(message.MessageId);

            commandUpdate.Command.Should().Be(sampleCommand);
            commandUpdate.Arguments.Should().HaveCount(2).And.ContainInOrder("a", "b");

            commandUpdate.Chat.Id.Should().Be(message.Chat.Id);
            commandUpdate.Chat.From.Should().BeSameAs(mappedUser);

            if (messageInGroup)
            {
                commandUpdate.Chat.Group.Should().BeSameAs(mappedGroup);
            }
            else
            {
                commandUpdate.Chat.Group.Should().BeNull();
            }
        }

        // TODO: Should be fixed later
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

            _mapperMock
                .Setup(x => x.Map<TelegramUser>(message.From))
                .Returns(new TelegramUser());

            var update = autoFixture.BuildDefaultUpdate()
                .With(x => x.Message, message)
                .Create();

            var resolver = CreateResolver();

            var updateResolved = resolver.TryResolveSupportedUpdate(update, out var supportedUpdate);

            updateResolved.Should().BeTrue();

            supportedUpdate.Should().BeOfType<TextMessageBotUpdate>()
                .Which.Text.Should().Be(text);
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

            var mappedUser = new TelegramUser();

            _mapperMock
                .Setup(x => x.Map<TelegramUser>(message.From))
                .Returns(mappedUser);

            var mappedGroup = new TelegramGroup();

            _mapperMock
                .Setup(x => x.Map<TelegramGroup>(message.Chat))
                .Returns(mappedGroup);

            var mappedContact = new TelegramContact();

            _mapperMock
                .Setup(x => x.Map<TelegramContact>(message.Contact))
                .Returns(mappedContact);

            var update = autoFixture.BuildDefaultUpdate()
                .With(x => x.Message, message)
                .Create();

            var resolver = CreateResolver();

            var updateResolved = resolver.TryResolveSupportedUpdate(update, out var supportedUpdate);

            updateResolved.Should().BeTrue();

            var contactUpdate = supportedUpdate.Should().BeOfType<ContactMessageBotUpdate>().Subject;

            contactUpdate.UpdateId.Should().Be(update.Id);
            contactUpdate.MessageId.Should().Be(message.MessageId);

            contactUpdate.Contact.Should().BeSameAs(mappedContact);

            contactUpdate.Chat.Id.Should().Be(message.Chat.Id);
            contactUpdate.Chat.From.Should().BeSameAs(mappedUser);

            if (messageInGroup)
            {
                contactUpdate.Chat.Group.Should().BeSameAs(mappedGroup);
            }
            else
            {
                contactUpdate.Chat.Group.Should().BeNull();
            }
        }

        private BotUpdateResolver CreateResolver() =>
            new BotUpdateResolver(_mapperMock.Object);
    }
}