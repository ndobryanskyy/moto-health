using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    internal abstract class BotUpdateBase : IBotUpdate
    {
        public int UpdateId { get; set; }
    }
}