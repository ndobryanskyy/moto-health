using AutoFixture;
using AutoFixture.Dsl;
using Telegram.Bot.Types;

namespace MotoHealth.Bot.Tests.Fixtures.Telegram
{
    internal static class UpdateAutoFixtureExtensions
    {
        public static IPostprocessComposer<Update> BuildDefaultUpdate(this Fixture fixture)
        {
            return fixture.Build<Update>()
                .OmitAutoProperties()
                .With(x => x.Id);
        }
    }
}