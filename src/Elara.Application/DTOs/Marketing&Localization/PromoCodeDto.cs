using Elara.Domain.Enums;

namespace Elara.Application.DTOs
{
    public class PromoCodeDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = null!;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; }
        public int TimesUsed { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
}
}