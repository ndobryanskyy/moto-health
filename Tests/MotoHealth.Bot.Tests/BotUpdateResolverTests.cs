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
using BotCommand = MotoHealth.Core.Bot.Updates.BotCommand;

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
        public void Should_Resolve_Unknown_Command_Update(bool messageInGroup)
        {
            const string unknownCommand = "/RANDOM_COMMAND";

            var autoFixture = new Fixture();

            var messageEntities = new[]
            {
                new MessageEntity
                {
                    Type = MessageEntityType.BotCommand,
                    Offset = 0,
                    Length = unknownCommand.Length
                }
            };

            var messageBuilder = messageInGroup
                ? autoFixture.BuildDefaultGroupMessage()
                : autoFixture.BuildDefaultPrivateMessage();

            var message = messageBuilder
                .With(x => x.Text, $"{unknownCommand}    a       b")
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

            commandUpdate.Command.Should().Be(BotCommand.Unknown);
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

        [Theory]
        [InlineData("/start", BotCommand.Start)]
        [InlineData("/info", BotCommand.About)]
        [InlineData("/dtp", BotCommand.ReportAccident)]
        public void Should_Resolve_Known_Command_Update(string messageText, BotCommand expectedBotCommand)
        {
            var autoFixture = new Fixture();

            var messageEntities = new[]
            {
                new MessageEntity
                {
                    Type = MessageEntityType.BotCommand,
                    Offset = 0,
                    Length = messageText.Length
                }
            };

            var message = autoFixture
                .BuildDefaultPrivateMessage()
                .With(x => x.Text, messageText)
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

            var commandUpdate = supportedUpdate.Should().BeOfType<CommandBotUpdate>().Subject;
                
            commandUpdate.Command.Should().Be(expectedBotCommand);
            commandUpdate.Arguments.Should().BeEmpty();
        }

        [Fact]
        public void Should_Resolve_Text_Message_Update_If_More_Than_One_MessageEntity_Present()
        {
            var autoFixture = new Fixture();

            var command1 = $"/{BotCommand.Start.ToString().ToLowerInvariant()}";
            var command2 = $"/{BotCommand.About.ToString().ToLowerInvariant()}";

            var text = $"{command1} {command2}";

            var messageEntities = new[]
            {
                new MessageEntity
                {
                    Type = MessageEntityType.BotCommand,
                    Offset = 0,
                    Length = command1.Length
                },

                new MessageEntity
                {
                    Type = MessageEntityType.BotCommand,
                    Offset = command1.Length + 1,
                    Length = command2.Length
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