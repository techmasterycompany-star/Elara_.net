using Elara.Domain.Enums;

namespace Elara.Application.DTOs
{
    public class CreatePromoCodeDto
    {
        public string Code { get; set; } = null!;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; }
    }
}