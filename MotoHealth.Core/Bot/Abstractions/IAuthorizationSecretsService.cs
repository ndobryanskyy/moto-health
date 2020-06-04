namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IAuthorizationSecretsService
    {
        bool VerifySubscriptionSecret(string secret);
    }
}