﻿using System.Threading;
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
        Task<BanOperationResult> BanUserAsync(int userId, CancellationToken cancellationToken);
        
        Task<BanOperationResult> UnbanUserAsync(int userId, CancellationToken cancellationToken);
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

        public async Task<BanOperationResult> BanUserAsync(int userId, CancellationToken cancellationToken)
            => await ChangeUserBannedStateAsync(userId, true, cancellationToken);

        public async Task<BanOperationResult> UnbanUserAsync(int userId, CancellationToken cancellationToken) 
            => await ChangeUserBannedStateAsync(userId, false, cancellationToken);

        private async Task<BanOperationResult> ChangeUserBannedStateAsync(int userId, bool desiredBanState, CancellationToken cancellationToken)
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

            _logger.LogDebug($"User {userId} current ban state: {state.UserBanned}");

            return BanOperationResult.Success;
        }
    }
}