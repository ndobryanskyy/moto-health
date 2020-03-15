using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IBotUpdateContext : IConversationContext
    {
        IBotUpdate Update { get; }
    }
}