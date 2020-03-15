using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public sealed class ChatContext : IChatContext
    {
        public ChatContext(
            long id, 
            bool isGroup, 
            string? username, 
            string? title)
        {
            Id = id;
            IsGroup = isGroup;
            Username = username;
            Title = title;
        }

        public long Id { get; }

        public bool IsGroup { get; }

        public string? Username { get; }

        public string? Title { get; }
    }
}