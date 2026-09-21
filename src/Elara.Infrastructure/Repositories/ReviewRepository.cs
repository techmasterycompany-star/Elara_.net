using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Review;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetByIdAsync(long reviewId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reviewId && !r.IsDeleted);
        }

        public async Task<Review?> GetByUserAndProductAsync(long userId, long productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId && !r.IsDeleted);
        }

        public async Task<PaginationQueryResult<Review>> GetProductReviewsAsync(long productId, ReviewQuery query)
        {
            var reviews = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId && !r.IsDeleted)
                .AsQueryable();

            if (query.Rating.HasValue)
            {
                reviews = reviews.Where(r => r.Rating == query.Rating.Value);
            }

            reviews = query.SortBy?.ToLower() switch
            {
                "oldest" => reviews.OrderBy(r => r.CreatedAt),
                "highest" => reviews.OrderByDescending(r => r.Rating).ThenBy(r => r.CreatedAt),
                "lowest" => reviews.OrderBy(r => r.Rating).ThenBy(r => r.CreatedAt),
                _ => reviews.OrderByDescending(r => r.CreatedAt) // newest
            };

            var totalCount = await reviews.CountAsync();
            var skip = (query.PageNumber - 1) * query.Limit;
            var items = await reviews.Skip(skip).Take(query.Limit).ToListAsync();

            return new PaginationQueryResult<Review>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<Review> AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task DeleteAsync(Review review)
        {
            review.IsDeleted = true;
            review.DeletedAt = DateTime.UtcNow;
            review.UpdatedAt = DateTime.UtcNow;
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasUserReviewedProductAsync(long userId, long productId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId && !r.IsDeleted);
        }

        public async Task<(double AverageRating, int TotalReviews, Dictionary<int, int> Distribution)> GetProductReviewStatsAsync(long productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId && !r.IsDeleted)
                .ToListAsync();

            var totalReviews = reviews.Count;
            var averageRating = totalReviews > 0 ? reviews.Average(r => r.Rating) : 0;
            
            var distribution = new Dictionary<int, int>
            {
                { 5, 0 }, { 4, 0 }, { 3, 0 }, { 2, 0 }, { 1, 0 }
            };

            foreach (var review in reviews)
            {
                if (distribution.ContainsKey(review.Rating))
                    distribution[review.Rating]++;
            }

            return (averageRating, totalReviews, distribution);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}