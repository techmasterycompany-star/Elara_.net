using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Review;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(long reviewId);
        Task<Review?> GetByUserAndProductAsync(long userId, long productId);
        Task<PaginationQueryResult<Review>> GetProductReviewsAsync(long productId, ReviewQuery query);
        Task<Review> AddAsync(Review review);
        Task<Review> UpdateAsync(Review review);
        Task DeleteAsync(Review review);
        Task<bool> HasUserReviewedProductAsync(long userId, long productId);
        Task<(double AverageRating, int TotalReviews, Dictionary<int, int> Distribution)> GetProductReviewStatsAsync(long productId);
        Task SaveChangesAsync();
    }
}