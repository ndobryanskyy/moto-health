namespace MotoHealth.Bot.Authorization
{
    internal sealed class AuthorizationSecretsOptions
    {
        public string SubscriptionSecret { get; set; } = string.Empty;

        public string BanSecret { get; set; } = string.Empty;
    }
}