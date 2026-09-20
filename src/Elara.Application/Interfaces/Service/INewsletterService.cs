using Elara.Application.DTOs;

namespace Elara.Application.Interfaces
{
    public interface INewsletterService
    {
        Task<SubscribeResultDto> SubscribeAsync(string email, long? userId, CancellationToken cancellationToken = default);
        Task<SubscribeResultDto> UnsubscribeAsync(string email, CancellationToken cancellationToken = default);
        Task<List<NewsletterSubscriberDto>> GetSubscribersAsync(CancellationToken cancellationToken = default);
        Task<SendCampaignResultDto> SendCampaignAsync(SendCampaignRequestDto request, CancellationToken cancellationToken = default);
    }
}