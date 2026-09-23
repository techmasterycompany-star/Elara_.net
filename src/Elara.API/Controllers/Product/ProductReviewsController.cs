using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Review;
using Elara.Application.Interfaces.Service;
using Elara.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Product
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ProductReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET /products/{productId}/reviews
        [HttpGet("{productId}/reviews")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductReviews(long productId, [FromQuery] ReviewQuery query)
        {
            var reviews = await _reviewService.GetProductReviewsWithStatsAsync(productId, query);
            var response = ApiResponse<ProductReviewsResponseDto>.SuccessResponse(reviews);
            return Ok(response);
        }

        // POST /products/{productId}/reviews
        [HttpPost("{productId}/reviews")]
        [Authorize]
        public async Task<IActionResult> CreateReview(long productId, [FromBody] CreateReviewDto dto)
        {
            var userId = User.GetAuthenticatedUserId();
            var review = await _reviewService.CreateReviewAsync(userId, productId, dto);
            var response = ApiResponse<ReviewResponseDto>.SuccessResponse(review);
            return Ok(response);
        }

        // PUT /products/{productId}/reviews/{reviewId}
        [HttpPut("{productId}/reviews/{reviewId}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(long productId, long reviewId, [FromBody] UpdateReviewDto dto)
        {
            var userId = User.GetAuthenticatedUserId();
            var review = await _reviewService.UpdateReviewAsync(userId, productId, reviewId, dto);
            var response = ApiResponse<ReviewResponseDto>.SuccessResponse(review);
            return Ok(response);
        }

        // DELETE /products/{productId}/reviews/{reviewId}
        [HttpDelete("{productId}/reviews/{reviewId}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(long productId, long reviewId)
        {
            var userId = User.GetAuthenticatedUserId();
            await _reviewService.DeleteReviewAsync(userId, productId, reviewId);
            var response = ApiResponse<object>.SuccessResponse(null);
            return Ok(response);
        }

        // GET /products/{productId}/reviews/can-review
        [HttpGet("{productId}/reviews/can-review")]
        [Authorize]
        public async Task<IActionResult> CanUserReview(long productId)
        {
            var userId = User.GetAuthenticatedUserId();
            var canReview = await _reviewService.CanUserReviewProductAsync(userId, productId);
            var response = ApiResponse<bool>.SuccessResponse(canReview);
            return Ok(response);
        }
    }
}