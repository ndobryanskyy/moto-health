﻿using System;
using FluentAssertions;
using Xunit;

namespace MotoHealth.Telegram.Tests
{
    public sealed class TelegramHtmlTests
    {
        [Fact]
        public void Link_Should_Create_Html_Anchor_Tag_With_AbsoluteUri()
        {
            const string title = "Google <inc>";
            var uri = new Uri("http://google.com", UriKind.Absolute);

            var link = TelegramHtml.Link(uri, title);

            link.Should().Be(@"<a href=""http://google.com/"">Google &lt;inc&gt;</a>");
        }

        [Fact]
        public void UserLink_Should_Create_Html_Anchor_Tag_With_Telegram_UserId()
        {
            const string title = "<me>";
            const long userId = 4_294_967_294; // int.MaxValue * 2

            var userLink = TelegramHtml.UserLink(userId, title);

            userLink.Should().Be(@"<a href=""tg://user?id=4294967294"">&lt;me&gt;</a>");
        }
    }
}