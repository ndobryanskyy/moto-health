using AutoFixture;
using AutoFixture.Dsl;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Bot.Tests.Fixtures.Telegram
{
    internal static class ChatAutoFixtureExtensions
    {
        public static IPostprocessComposer<Chat> BuildDefaultPrivateChat(this Fixture fixture, User sender)
        {
            return ApplyDefaultConfiguration(fixture.Build<Chat>(), sender)
                .With(x => x.Type, ChatType.Private);
        }

        public static IPostprocessComposer<Chat> BuildDefaultGroupChat(this Fixture fixture, User sender)
        {
            return ApplyDefaultConfiguration(fixture.Build<Chat>(), sender)
                .With(x => x.Type, ChatType.Group)
                .With(x => x.Title);
        }

        private static IPostprocessComposer<Chat> ApplyDefaultConfiguration(IPostprocessComposer<Chat> chat, User sender)
        {
            return chat
                .OmitAutoProperties()
                .With(x => x.Id)
                .With(x => x.FirstName, sender.FirstName)
                .With(x => x.LastName, sender.LastName)
                .With(x => x.Username, sender.Username);
        }
    }
}