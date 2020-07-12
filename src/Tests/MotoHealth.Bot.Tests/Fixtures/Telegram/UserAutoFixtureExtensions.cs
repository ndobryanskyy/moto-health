using AutoFixture;
using AutoFixture.Dsl;
using Telegram.Bot.Types;

namespace MotoHealth.Bot.Tests.Fixtures.Telegram
{
    internal static class UserAutoFixtureExtensions
    {
        public static IPostprocessComposer<User> BuildDefaultUser(this Fixture fixture)
        {
            return fixture.Build<User>()
                .OmitAutoProperties()
                .With(x => x.Id)
                .With(x => x.FirstName)
                .With(x => x.IsBot, false)
                .With(x => x.LanguageCode, "ru");
        }
    }
}