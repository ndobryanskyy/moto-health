using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MotoHealth.Functions.Authorization
{
    public interface IAuthorizationService
    {
        bool VerifySubscriptionSecret(string secret);
    }

    internal sealed class AuthorizationService : IAuthorizationService
    {
        private readonly ILogger<AuthorizationService> _logger;
        private readonly string _subscriptionSecret;

        public AuthorizationService(
            ILogger<AuthorizationService> logger,
            IOptions<AuthorizationOptions> options)
        {
            _logger = logger;
            _subscriptionSecret = options.Value.SubscriptionSecret;
        }

        public bool VerifySubscriptionSecret(string secret)
        {
            if (string.IsNullOrEmpty(_subscriptionSecret))
            {
                _logger.LogError("Subscription secret is not initialized. Authorization will always fail");

                return false;
            }

            var secretValid = secret == _subscriptionSecret;

            if (!secretValid)
            {
                _logger.LogWarning($"Secret '{secret}' mismatched");
            }

            return secretValid;
        }
    }
}