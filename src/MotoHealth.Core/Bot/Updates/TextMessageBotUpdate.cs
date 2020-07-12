using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    internal sealed class TextMessageBotUpdate : MessageBotUpdateBase, ITextMessageBotUpdate
    {
        public string Text { get; set; } = default!;
    }
}