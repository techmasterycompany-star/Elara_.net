using Elara.Application.DTOs;
using Elara.Application.Interfaces;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;
using Elara.Domain.Enums;

namespace Elara.Application.Services
{
    public class ReferralService : IReferralService
    {
        private const int SignupRewardPoints = 50;
        private readonly IReferralRepository _referralRepository;
        private readonly IEmailCampaignSender _emailSender;

        public ReferralService(
            IReferralRepository referralRepository,
            IEmailCampaignSender emailSender)
        {
            _referralRepository = referralRepository;
            _emailSender = emailSender;
        }

        public ReferralCodeDto GetMyCode(long userId)
        {
            return new ReferralCodeDto
            {
                Code = EncodeUserId(userId)
            };
        }

        public async Task<InviteFriendResultDto> InviteFriendAsync(
            long senderUserId,
            string senderName,
            string friendEmail,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(friendEmail) || !friendEmail.Contains('@'))
                return new InviteFriendResultDto
                {
                    IsSuccess = false
                };

            var code = EncodeUserId(senderUserId);

            var subject = $"{senderName} invited you to join Elara!";
            var body = $"""
                <p>Hi there,</p>
                <p><strong>{senderName}</strong> thinks you'd love Elara and wants to invite you to join.</p>
                <p>Sign up using referral code <strong>{code}</strong> and you'll both earn reward points once you complete your first order.</p>
                <p>See you soon!</p>
                """;

            await _emailSender.SendAsync(new List<string> { friendEmail }, subject, body, cancellationToken);

            return new InviteFriendResultDto { IsSuccess = true };
        }

        public async Task<ApplyReferralResultDto> ApplyAsync(
            long referredUserId,
            string code,
            CancellationToken cancellationToken = default)
        {
            var referrerUserId = DecodeUserId(code);

            if (referrerUserId == null)
                return Invalid("This referral code is invalid.");

            if (referrerUserId == referredUserId)
                return Invalid("You cannot use your own referral code.");

            var existing = await _referralRepository.GetByReferredUserIdAsync(referredUserId, cancellationToken);
            if (existing != null)
                return Invalid("A referral code has already been applied to this account.");

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
            return "REF" + obfuscated.ToString("X");
        }

        private static long? DecodeUserId(string code)
        {
            if (string.IsNullOrWhiteSpace(code) || !code.StartsWith("REF", StringComparison.OrdinalIgnoreCase))
                return null;

            try
            {
                var raw = code[3..];
                var obfuscated = Convert.ToInt64(raw, 16);
                var userId = (obfuscated - 104729) / 7919;
                return userId;
            }
            catch
            {
                return null;
            }
        }

        private static ApplyReferralResultDto Invalid(string message) => new()
        {
            IsSuccess = false,
            ErrorMessage = message
        };
    }
}