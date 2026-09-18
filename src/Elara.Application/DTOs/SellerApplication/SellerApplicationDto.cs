using Elara.Domain.Enums;

namespace Elara.Application.DTOs.SellerApplication
{
    public class SellerApplicationDto
    {
        public long Id { get; set; }
        public string StoreName { get; set; } = null!;
        public string StoreDescription { get; set; } = null!;
        public SellerApplicationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
