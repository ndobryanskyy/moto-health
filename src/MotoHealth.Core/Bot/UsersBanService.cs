using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot
{
    public enum BanOperationResult
    {
        UserIsUnknown,
        CurrentStateMatchesDesired,
        Success
    }

    public interface IUsersBanService
    {
        Task<BanOperationResult> BanUserAsync(long userId, CancellationToken cancellationToken);
        
        Task<BanOperationResult> UnbanUserAsync(long userId, CancellationToken cancellationToken);

        ValueTask<bool> CheckIfUserIsBannedAsync(long userId, CancellationToken cancellationToken);
    }

    internal sealed class UsersBanService : IUsersBanService
    {
        private readonly ILogger<UsersBanService> _logger;
        private readonly IChatStatesRepository _chatStatesRepository;

        public UsersBanService(
            ILogger<UsersBanService> logger,
            IChatStatesRepository chatStatesRepository)
        {
            _logger = logger;
            _chatStatesRepository = chatStatesRepository;
        }

        public async Task<BanOperationResult> BanUserAsync(long userId, CancellationToken cancellationToken)
            => await ChangeUserBannedStateAsync(userId, true, cancellationToken);

        public async Task<BanOperationResult> UnbanUserAsync(long userId, CancellationToken cancellationToken) 
            => await ChangeUserBannedStateAsync(userId, false, cancellationToken);

        public async ValueTask<bool> CheckIfUserIsBannedAsync(long userId, CancellationToken cancellationToken)
        {
            var state = await _chatStatesRepository.GetForChatAsync(userId, cancellationToken)
                        ?? throw new InvalidOperationException($"Chat for user '{userId}' was not found");

            return state.UserBanned;
        }

        private async Task<BanOperationResult> ChangeUserBannedStateAsync(long userId, bool desiredBanState, CancellationToken cancellationToken)
        {
            var state = await _chatStatesRepository.GetForChatAsync(userId, cancellationToken);
            if (state == null)
            {
                _logger.LogWarning($"Tried to ban / unban unknown user {userId}");

                return BanOperationResult.UserIsUnknown;
            }

            if (state.UserBanned == desiredBanState)
            {
                _logger.LogWarning($"User {userId} is already in desired ban state {desiredBanState}");

                return BanOperationResult.CurrentStateMatchesDesired;
            }

            state.UserBanned = desiredBanState;
            await _chatStatesRepository.UpdateAsync(state, cancellationToken);

            _logger.LogDebug($"User {userId} new ban state: {state.UserBanned}");

            return BanOperationResult.Success;
        }
    }
}