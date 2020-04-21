using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public sealed class TextMessageBotUpdate : MessageBotUpdateBase, ITextMessageBotUpdate
    {
        public string Text { get; internal set; } = string.Empty;
    }
}