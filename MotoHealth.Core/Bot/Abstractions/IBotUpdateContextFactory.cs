using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IBotUpdateContextFactory
    {
        IBotUpdateContext CreateForUpdate(IBotUpdate update);
    }
}