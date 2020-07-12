namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface IChatUpdate : IBotUpdate, IBelongsToChat, IHasSender
    {
    }
}