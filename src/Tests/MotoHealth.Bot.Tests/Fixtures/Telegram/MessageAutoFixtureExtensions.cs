using AutoFixture;
using AutoFixture.Dsl;
using Telegram.Bot.Types;

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