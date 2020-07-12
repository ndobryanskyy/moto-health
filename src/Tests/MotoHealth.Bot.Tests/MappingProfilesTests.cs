using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MotoHealth.Bot.Tests.Fixtures;
using Xunit;

namespace MotoHealth.Bot.Tests
{
    public sealed class MappingProfilesTests : IClassFixture<TestBotAppFactory>
    {
        private readonly TestBotAppFactory _appFactory;

        public MappingProfilesTests(TestBotAppFactory appFactory)
        {
            _appFactory = appFactory;
        }

        [Fact]
        public void All_Mapping_Profiles_Should_Be_Valid()
        {
            var mapper = _appFactory.Server.Services.GetRequiredService<IMapper>();

            mapper.ConfigurationProvider
                .Invoking(x => x.AssertConfigurationIsValid())
                .Should()
                .NotThrow();
        }
    }
}
