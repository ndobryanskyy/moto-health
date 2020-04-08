using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatFactory
    {
        IChatController CreateController(IBotUpdateContext updateContext, IChatState state);

        IChatState CreateDefaultState(long chatId, int userId);

        IBotUpdateContext CreateUpdateContext(IBotUpdate update);
    }
}