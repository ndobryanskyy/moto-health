using AutoFixture;
using AutoFixture.Dsl;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Bot.Tests.Fixtures.Telegram
{
    internal static class MessageAutoFixtureExtensions
    {
        public static IPostprocessComposer<Message> BuildDefaultPrivateMessage(this Fixture fixture)
        {
            var sender = fixture.BuildDefaultUser().Create();
            var chat = fixture.BuildDefaultPrivateChat(sender).Create();

            return ApplyDefaultConfiguration(fixture.Build<Message>(), sender, chat);
        }

        public static IPostprocessComposer<Message> BuildDefaultGroupMessage(this Fixture fixture)
        {
            var sender = fixture.BuildDefaultUser().Create();
            var chat = fixture.BuildDefaultGroupChat(sender).Create();

            return ApplyDefaultConfiguration(fixture.Build<Message>(), sender, chat);
        }

        public static IPostprocessComposer<Message> WithCommand(
            this IPostprocessComposer<Message> message,
            string command, 
            string? arguments = null)
        {
            var text = $"{command}{arguments}";
            var commandEntity = new MessageEntity
            {
                Offset = 0,
                Length = command.Length,
                Type = MessageEntityType.BotCommand
            };

            return message
                .With(x => x.Text, text)
                .With(x => x.Entities, new [] { commandEntity });
        }

        private static IPostprocessComposer<Message> ApplyDefaultConfiguration(
            IPostprocessComposer<Message> message, User sender, Chat chat)
        {
            return message
                .OmitAutoProperties()
                .With(x => x.MessageId)
                .With(x => x.Date)
                .With(x => x.From, sender)
                .With(x => x.Chat, chat);
        }
    }
}