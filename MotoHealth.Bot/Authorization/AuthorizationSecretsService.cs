using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Bot.Authorization
{
    internal sealed class AuthorizationSecretsService : IAuthorizationSecretsService
    {
        private readonly ILogger<AuthorizationSecretsService> _logger;
        private readonly string _subscriptionSecret;

        public AuthorizationSecretsService(
            ILogger<AuthorizationSecretsService> logger,
            IOptions<AuthorizationSecretsOptions> options)
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