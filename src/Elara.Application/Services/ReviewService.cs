using AutoMapper;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Review;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;

namespace Elara.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewRepository reviewRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<ReviewResponseDto>> GetProductReviewsAsync(long productId, ReviewQuery query)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new NotFoundException("Product not found.");

            var result = await _reviewRepository.GetProductReviewsAsync(productId, query);
            var reviewDtos = _mapper.Map<List<ReviewResponseDto>>(result.Items);

            return new PaginatedResponse<ReviewResponseDto>
            {
                Data = reviewDtos,
                PageNumber = query.PageNumber,
                Limit = query.Limit,
                TotalCount = result.TotalCount,
                TotalPages = (int)Math.Ceiling((double)result.TotalCount / query.Limit)
            };
        }

        public async Task<ProductReviewsResponseDto> GetProductReviewsWithStatsAsync(long productId, ReviewQuery query)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new NotFoundException("Product not found.");

            var reviewsResponse = await GetProductReviewsAsync(productId, query);
            var stats = await _reviewRepository.GetProductReviewStatsAsync(productId);

            return new ProductReviewsResponseDto
            {
                ProductId = productId,
                ProductName = product.Name,
                AverageRating = Math.Round(stats.AverageRating, 1),
                TotalReviews = stats.TotalReviews,
                RatingDistribution = stats.Distribution,
                Reviews = reviewsResponse.Data.ToList()
            };
        }

        public async Task<ReviewResponseDto> CreateReviewAsync(long userId, long productId, CreateReviewDto dto)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new NotFoundException("Product not found.");

            // Check if user already reviewed this product
            var hasReviewed = await _reviewRepository.HasUserReviewedProductAsync(userId, productId);
            if (hasReviewed)
                throw new ConflictException("You have already reviewed this product.");

            // Verify user purchased the product
            var hasPurchased = await _orderRepository.HasUserPurchasedProductAsync(userId, productId);
            if (!hasPurchased)
                throw new BadRequestException("You can only review products you have purchased and received.");

            var review = new Review
            {
                UserId = userId,
                ProductId = productId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);

            // Reload with user info
            var createdReview = await _reviewRepository.GetByIdAsync(review.Id);
            return _mapper.Map<ReviewResponseDto>(createdReview!);
        }

        public async Task<ReviewResponseDto> UpdateReviewAsync(long userId, long productId, long reviewId, UpdateReviewDto dto)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new NotFoundException("Review not found.");

            if (review.ProductId != productId)
                throw new BadRequestException("Review does not belong to this product.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You can only update your own reviews.");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            await _reviewRepository.UpdateAsync(review);

            var updatedReview = await _reviewRepository.GetByIdAsync(reviewId);
            return _mapper.Map<ReviewResponseDto>(updatedReview!);
        }

        public async Task DeleteReviewAsync(long userId, long productId, long reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new NotFoundException("Review not found.");

            if (review.ProductId != productId)
                throw new BadRequestException("Review does not belong to this product.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own reviews.");

            await _reviewRepository.DeleteAsync(review);
        }

        public async Task<bool> CanUserReviewProductAsync(long userId, long productId)
        {
            var hasReviewed = await _reviewRepository.HasUserReviewedProductAsync(userId, productId);
            if (hasReviewed)
                return false;

            var hasPurchased = await _orderRepository.HasUserPurchasedProductAsync(userId, productId);
            return hasPurchased;
        }
    }
}