using FluentAssertions;
using MotoHealth.Core.Extensions;
using Xunit;

namespace MotoHealth.Bot.Tests
{
    /// <summary>
    /// Escaping required as per documentation of Telegram. Refer too https://core.telegram.org/bots/api#markdownv2-style
    /// </summary>
    public sealed class MarkdownExtensionsTests
    {
        [Fact]
        public void Should_Escape_Backslash()
        {
            var escaped = @"\Escape\\ \me\ \\please\".EscapeForMarkdown();

            escaped.Should().Be(@"\\Escape\\\\ \\me\\ \\\\please\\");
        }

        [Fact]
        public void Should_Escape_Underscore()
        {
            var escaped = "_Escape__ _me_ __please_".EscapeForMarkdown();

            escaped.Should().Be(@"\_Escape\_\_ \_me\_ \_\_please\_");
        }

        [Fact]
        public void Should_Escape_Asterisk()
        {
            var escaped = "*Escape** *me* **please*".EscapeForMarkdown();

            escaped.Should().Be(@"\*Escape\*\* \*me\* \*\*please\*");
        }

        [Fact]
        public void Should_Escape_Square_Brackets()
        {
            var escaped = "]Escape[[ [me] ]]please[".EscapeForMarkdown();

            escaped.Should().Be(@"\]Escape\[\[ \[me\] \]\]please\[");
        }

        [Fact]
        public void Should_Escape_Parentheses()
        {
            var escaped = ")Escape(( (me) ))please(".EscapeForMarkdown();

            escaped.Should().Be(@"\)Escape\(\( \(me\) \)\)please\(");
        }

        [Fact]
        public void Should_Escape_Tilde()
        {
            var escaped = "~Escape~~ ~me~ ~~please~".EscapeForMarkdown();

            escaped.Should().Be(@"\~Escape\~\~ \~me\~ \~\~please\~");
        }

        [Fact]
        public void Should_Escape_Backticks()
        {
            var escaped = "`Escape`` `me` ``please`".EscapeForMarkdown();

            escaped.Should().Be(@"\`Escape\`\` \`me\` \`\`please\`");
        }

        [Fact] public void Should_Escape_Closing_Angle_Bracket()
        {
            var escaped = ">Escape>> >me> >>please>".EscapeForMarkdown();

            escaped.Should().Be(@"\>Escape\>\> \>me\> \>\>please\>");
        }

        [Fact]
        public void Should_Escape_Number_Sign()
        {
            var escaped = "#Escape## #me# ##please#".EscapeForMarkdown();

            escaped.Should().Be(@"\#Escape\#\# \#me\# \#\#please\#");
        }

        [Fact]
        public void Should_Escape_Plus_Sign()
        {
            var escaped = "+Escape++ +me+ ++please+".EscapeForMarkdown();

            escaped.Should().Be(@"\+Escape\+\+ \+me\+ \+\+please\+");
        }

        [Fact]
        public void Should_Escape_Dashes()
        {
            var escaped = "-Escape-- -me- --please-".EscapeForMarkdown();

            escaped.Should().Be(@"\-Escape\-\- \-me\- \-\-please\-");
        }

        [Fact]
        public void Should_Escape_Equality_Sign()
        {
            var escaped = "=Escape== =me= ==please=".EscapeForMarkdown();

            escaped.Should().Be(@"\=Escape\=\= \=me\= \=\=please\=");
        }

        [Fact]
        public void Should_Escape_Curly_Brackets()
        {
            var escaped = "}Escape{{ {me} }}please{".EscapeForMarkdown();

            escaped.Should().Be(@"\}Escape\{\{ \{me\} \}\}please\{");
        }

        [Fact]
        public void Should_Escape_Dots()
        {
            var escaped = ".Escape.. .me. ..please.".EscapeForMarkdown();

            escaped.Should().Be(@"\.Escape\.\. \.me\. \.\.please\.");
        }

        [Fact]
        public void Should_Escape_Exclamations()
        {
            var escaped = "!Escape!! !me! !!please!".EscapeForMarkdown();

            escaped.Should().Be(@"\!Escape\!\! \!me\! \!\!please\!");
        }

        [Fact]
        public void Should_Escape_Special_Characters_Mixed()
        {
            var escaped = "Hello [BB-8]! Nice to meet you {~`~}. Head that way |==\\> (+ remember #never_look_back)".EscapeForMarkdown();

            escaped.Should().Be(@"Hello \[BB\-8\]\! Nice to meet you \{\~\`\~\}\. Head that way \|\=\=\\\> \(\+ remember \#never\_look\_back\)");
        }
    }
}