using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace MotoHealth.Functions.Tests
{
    public sealed class FunctionsMappingProfilesTests
    {
        [Fact]
        public void All_Mapping_Profiles_Should_Be_Valid()
        {
            var startup = new FunctionsStartup();
            var functionsHost = new HostBuilder()
                .ConfigureWebJobs(startup.Configure)
                .Build();

            var mapper = functionsHost.Services.GetRequiredService<IMapper>();
            mapper
                .Invoking(x => x.ConfigurationProvider.AssertConfigurationIsValid())
                .Should()
                .NotThrow();
        }
    }
}
