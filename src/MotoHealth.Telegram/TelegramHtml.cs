using System;
using MotoHealth.Telegram.Extensions;

namespace MotoHealth.Telegram
{
    public static class TelegramHtml
    {
        public static string Link(Uri href, string title) => @$"<a href=""{href.AbsoluteUri}"">{title.HtmlEscaped()}</a>";

        public static string UserLink(long userId, string title) => $@"<a href=""tg://user?id={userId:D}"">{title.HtmlEscaped()}</a>";
    }
}