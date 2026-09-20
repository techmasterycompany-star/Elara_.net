using Elara.Application.DTOs;
using Elara.Application.Interfaces;
using Elara.Domain.Entities;
using Elara.Domain.Enums;

namespace Elara.Application.Services
{
    public class ReferralService : IReferralService
    {
        private const int SignupRewardPoints = 50;
        private readonly IReferralRepository _referralRepository;

        public ReferralService(IReferralRepository referralRepository)
        {
            _referralRepository = referralRepository;
        }

        public ReferralCodeDto GetMyCode(long userId)
        {
            return new ReferralCodeDto
            {
                Code = EncodeUserId(userId)
            };
        }

        public async Task<ApplyReferralResultDto> ApplyAsync(
            long referredUserId,
            string code,
            CancellationToken cancellationToken = default)
        {
            var referrerUserId = DecodeUserId(code);

            if (referrerUserId == null)
                return Invalid("REFERRAL_CODE_INVALID", "This referral code is invalid.");

            if (referrerUserId == referredUserId)
                return Invalid("REFERRAL_SELF_NOT_ALLOWED", "You cannot use your own referral code.");

            var existing = await _referralRepository.GetByReferredUserIdAsync(referredUserId, cancellationToken);
            if (existing != null)
                return Invalid("REFERRAL_ALREADY_USED", "A referral code has already been applied to this account.");

            var referral = new Referral
            {
                ReferrerUserId = referrerUserId.Value,
                ReferredUserId = referredUserId,
                ReferralCode = code,
                Status = ReferralStatus.Completed,
                RewardPoints = SignupRewardPoints,
                CreatedAt = DateTime.UtcNow
            };

            await _referralRepository.AddAsync(referral, cancellationToken);

            // NOTE: once the Loyalty module exists, call into it here to actually
            // credit SignupRewardPoints to the referrer as a Transaction (Earned).

            return new ApplyReferralResultDto
            {
                IsSuccess = true,
                RewardPoints = SignupRewardPoints
            };
        }

        public async Task<List<ReferralHistoryDto>> GetHistoryAsync(
            long referrerUserId,
            CancellationToken cancellationToken = default)
        {
            var referrals = await _referralRepository.GetByReferrerUserIdAsync(referrerUserId, cancellationToken);

            return referrals.Select(r => new ReferralHistoryDto
            {
                ReferredUserId = r.ReferredUserId,
                Status = r.Status.ToString(),
                RewardPoints = r.RewardPoints,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        private static string EncodeUserId(long userId)
        {
            var obfuscated = (userId * 7919) + 104729;
            return "REF" + Convert.ToString(obfuscated, 36).ToUpperInvariant();
        }

        private static long? DecodeUserId(string code)
        {
            if (string.IsNullOrWhiteSpace(code) || !code.StartsWith("REF", StringComparison.OrdinalIgnoreCase))
                return null;

            try
            {
                var raw = code[3..];
                var obfuscated = Convert.ToInt64(raw, 36);
                var userId = (obfuscated - 104729) / 7919;
                return userId;
            }
            catch
            {
                return null;
            }
        }

        private static ApplyReferralResultDto Invalid(string errorCode, string message) => new()
        {
            IsSuccess = false,
            ErrorCode = errorCode,
            ErrorMessage = message
        };
    }
}