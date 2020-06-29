namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IWaterfallDialogState
    {
        public string InstanceId { get; }

        public int Version { get; }

        public int CurrentStep { get; set; }
    }
}