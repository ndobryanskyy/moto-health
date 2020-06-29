using System;

namespace MotoHealth.Telegram
{
    public static class TelegramHtml
    {
        public static string Link(Uri href, string escapedTitle) => @$"<a href=""{href.AbsoluteUri}"">{escapedTitle}</a>";

        public static string UserLink(int userId, string escapedTitle) => $@"<a href=""tg://user?id={userId}"">{escapedTitle}</a>";
    }
}