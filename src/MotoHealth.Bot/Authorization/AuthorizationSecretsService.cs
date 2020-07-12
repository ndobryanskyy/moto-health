using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Bot.Authorization
{
    internal sealed class AuthorizationSecretsService : IAuthorizationSecretsService
    {
        private readonly ILogger<AuthorizationSecretsService> _logger;
        private readonly AuthorizationSecretsOptions _secrets;

        public AuthorizationSecretsService(
            ILogger<AuthorizationSecretsService> logger,
            IOptions<AuthorizationSecretsOptions> options)
        {
            _logger = logger;
            _secrets = options.Value;
        }

        public bool VerifySubscriptionSecret(string secret) 
            => VerifySecret(nameof(_secrets.SubscriptionSecret), _secrets.SubscriptionSecret, secret);

        public bool VerifyBanSecret(string secret)
            => VerifySecret(nameof(_secrets.BanSecret), _secrets.BanSecret, secret);

        private bool VerifySecret(string secretName, string secret, string input)
        {
            var secretValid = secret == input;

            if (!secretValid)
            {
                _logger.LogWarning($"Secret {secretName} mismatched for value '{input}'");
            }

            return secretValid;
        }
    }
}