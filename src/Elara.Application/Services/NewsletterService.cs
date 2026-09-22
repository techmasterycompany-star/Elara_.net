using Elara.Application.DTOs;
using Elara.Application.Interfaces;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;

namespace Elara.Application.Services
{
    public class NewsletterService : INewsletterService
    {
        private readonly INewsletterSubscriptionRepository _repository;
        private readonly IEmailCampaignSender _emailSender;

        public NewsletterService(
            INewsletterSubscriptionRepository repository,
            IEmailCampaignSender emailSender)
        {
            _repository = repository;
            _emailSender = emailSender;
        }

        public async Task<SubscribeResultDto> SubscribeAsync(string email, long? userId, CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByEmailAsync(email, cancellationToken);

            if (existing != null)
            {
                if (existing.IsSubscribed)
                    return Invalid("ALREADY_SUBSCRIBED", "This email is already subscribed.");

                existing.IsSubscribed = true;
                existing.SubscribedAt = DateTime.UtcNow;
                existing.UnsubscribedAt = null;
                await _repository.UpdateAsync(existing, cancellationToken);
                return new SubscribeResultDto { IsSuccess = true };
            }

            var subscription = new NewsletterSubscription
            {
                UserId = userId,
                Email = email,
                IsSubscribed = true,
                SubscribedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(subscription, cancellationToken);
            return new SubscribeResultDto { IsSuccess = true };
        }

        public async Task<SubscribeResultDto> UnsubscribeAsync(string email, CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByEmailAsync(email, cancellationToken);

            if (existing == null || !existing.IsSubscribed)
                return Invalid("NOT_SUBSCRIBED", "This email is not currently subscribed.");

            existing.IsSubscribed = false;
            existing.UnsubscribedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(existing, cancellationToken);

            return new SubscribeResultDto { IsSuccess = true };
        }

        public async Task<List<NewsletterSubscriberDto>> GetSubscribersAsync(CancellationToken cancellationToken = default)
        {
            var subscribers = await _repository.GetAllSubscribedAsync(cancellationToken);

            return subscribers.Select(s => new NewsletterSubscriberDto
            {
                Email = s.Email,
                IsSubscribed = s.IsSubscribed,
                SubscribedAt = s.SubscribedAt
            }).ToList();
        }

        public async Task<SendCampaignResultDto> SendCampaignAsync(SendCampaignRequestDto request, CancellationToken cancellationToken = default)
        {
            var subscribers = await _repository.GetAllSubscribedAsync(cancellationToken);
            var emails = subscribers.Select(s => s.Email).ToList();

            if (emails.Count == 0)
            {
                return new SendCampaignResultDto
                {
                    IsSuccess = false,
                    RecipientsCount = 0,
                    ErrorMessage = "No active subscribers to send to."
                };
            }

            await _emailSender.SendAsync(emails, request.Subject, request.Body, cancellationToken);

            return new SendCampaignResultDto
            {
                IsSuccess = true,
                RecipientsCount = emails.Count
            };
        }

        private static SubscribeResultDto Invalid(string errorCode, string message) => new()
        {
            IsSuccess = false,
            ErrorCode = errorCode,
            ErrorMessage = message
        };
    }
}