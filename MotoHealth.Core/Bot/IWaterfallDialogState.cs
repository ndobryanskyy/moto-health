namespace MotoHealth.Core.Bot
{
    public interface IWaterfallDialogState
    {
        public int Version { get; }

        public int CurrentStep { get; set; }
    }
}