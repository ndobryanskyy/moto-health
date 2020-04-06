namespace MotoHealth.Infrastructure.UpdatesQueue
{
    public sealed class UpdatesQueueOptions
    {
        public string ConnectionString { get; set; } = string.Empty;

        public int MaxConcurrentHandlers { get; set; } = 1;

        public int MessageWaitTimeoutInSeconds { get; set; } = 60;
    }
}