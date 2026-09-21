using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Review;

namespace Elara.Application.Interfaces.Service
{
    public interface IReviewService
    {
        Task<PaginatedResponse<ReviewResponseDto>> GetProductReviewsAsync(long productId, ReviewQuery query);
        Task<ProductReviewsResponseDto> GetProductReviewsWithStatsAsync(long productId, ReviewQuery query);
        Task<ReviewResponseDto> CreateReviewAsync(long userId, long productId, CreateReviewDto dto);
        Task<ReviewResponseDto> UpdateReviewAsync(long userId, long productId, long reviewId, UpdateReviewDto dto);
        Task DeleteReviewAsync(long userId, long productId, long reviewId);
        Task<bool> CanUserReviewProductAsync(long userId, long productId);
    }
}