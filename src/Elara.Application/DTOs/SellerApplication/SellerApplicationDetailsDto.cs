using Elara.Domain.Entities;
using Elara.Domain.Enums;

namespace Elara.Application.DTOs.SellerApplication
{
    public class SellerApplicationDetailsDto
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public string StoreName { get; set; } = null!;
        public string StoreDescription { get; set; } = null!;

        public SellerApplicationStatus Status { get; set; }
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
