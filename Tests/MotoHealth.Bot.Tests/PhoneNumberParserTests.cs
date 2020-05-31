using FluentAssertions;
using MotoHealth.Core.Bot;
using Xunit;

namespace MotoHealth.Bot.Tests
{
    public sealed class PhoneNumberParserTests
    {
        private readonly PhoneNumberParser _parser;

        public PhoneNumberParserTests()
        {
            _parser = new PhoneNumberParser();
        }

        [Theory]
        [InlineData("380501234567")]
        [InlineData("+380501234567")]
        public void Should_Parse_Number_With_County_Code(string phoneNumber)
        {
            _parser.TryParse(phoneNumber, out var parsedPhoneNumber).Should().BeTrue();
            parsedPhoneNumber.Should().Be("+380501234567");
        }

        [Fact]
        public void Should_Parse_Number_Without_Country_Code()
        {
            _parser.TryParse("501234567", out var parsedPhoneNumber).Should().BeTrue();
            parsedPhoneNumber.Should().Be("+380501234567");
        }

        [Fact]
        public void Should_Parse_Number_Without_Country_Code_With_Leading_Zero()
        {
            _parser.TryParse("0501234567", out var parsedPhoneNumber).Should().BeTrue();
            parsedPhoneNumber.Should().Be("+380501234567");
        }

        [Theory]
        [InlineData("+380 (50) 1234567")]
        [InlineData("+380-(50)-123-45-67")]
        [InlineData("+380 50 123 45 67")]
        [InlineData("0(50) 123 45 67")]
        [InlineData("050-1234567")]
        [InlineData("(50) 1234567")]
        [InlineData("50-1234567")]
        public void Should_Ignore_Dashes_Spaces_And_Parenthesis_When_Parsing(string phoneNumber)
        {
            _parser.TryParse(phoneNumber, out var parsedPhoneNumber).Should().BeTrue();
            parsedPhoneNumber.Should().Be("+380501234567");
        }

        [Fact]
        public void Should_Not_Parse_Number_With_Leading_Eight()
        {
            _parser.TryParse("80501234567", out _).Should().BeFalse();
        }

        [Theory]
        [InlineData("+0501234567")]
        [InlineData("+3805012345678")]
        [InlineData("3805012345678")]
        [InlineData("05012345678")]
        [InlineData("05012345")]
        [InlineData("Not a phone number")]
        public void Should_Not_Parse_Invalid_Number(string phoneNumber)
        {
            _parser.TryParse(phoneNumber, out _).Should().BeFalse();
        }
    }
}