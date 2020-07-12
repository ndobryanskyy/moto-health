using FluentAssertions;
using MotoHealth.Telegram.Extensions;
using Xunit;

namespace MotoHealth.Telegram.Tests
{
    /// <summary>
    /// Escaping required as per documentation of Telegram. Refer to https://core.telegram.org/bots/api#markdownv2-style
    /// </summary>
    public sealed class EscapingExtensionsTests
    {
        [Fact]
        public void Should_Escape_LessThan()
        {
            var escaped = @"<Escape<< <me< <<please< <<".HtmlEscaped();

            escaped.Should().Be(@"&lt;Escape&lt;&lt; &lt;me&lt; &lt;&lt;please&lt; &lt;&lt;");
        }

        [Fact]
        public void Should_Escape_GreaterThan()
        {
            var escaped = @">Escape>> >me> >>please> >>".HtmlEscaped();

            escaped.Should().Be(@"&gt;Escape&gt;&gt; &gt;me&gt; &gt;&gt;please&gt; &gt;&gt;");
        }

        [Fact]
        public void Should_Escape_Ampersand()
        {
            var escaped = @"&Escape&& &me& &&please& &&".HtmlEscaped();

            escaped.Should().Be(@"&amp;Escape&amp;&amp; &amp;me&amp; &amp;&amp;please&amp; &amp;&amp;");
        }

        [Fact]
        public void Should_Escape_Combined()
        {
            var escaped = @"<b>Will be escaped</b> & this too.".HtmlEscaped();

            escaped.Should().Be(@"&lt;b&gt;Will be escaped&lt;/b&gt; &amp; this too.");
        }
    }
}