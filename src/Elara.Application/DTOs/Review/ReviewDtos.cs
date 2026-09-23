using System.ComponentModel.DataAnnotations;

namespace Elara.Application.DTOs.Review
{
    public class CreateReviewDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        [MaxLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters")]
        public string Comment { get; set; } = null!;
    }

    public class UpdateReviewDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        [MaxLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters")]
        public string Comment { get; set; } = null!;
    }

    public class ReviewResponseDto
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ProductReviewsResponseDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
        public List<ReviewResponseDto> Reviews { get; set; } = new();
    }

    public class ReviewQuery
    {
        public int PageNumber { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public int? Rating { get; set; }
        public string? SortBy { get; set; } = "newest"; // newest, oldest, highest, lowest
    }
}