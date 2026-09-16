using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Order
{
    public class PromoCodeSummaryDto
    {
        public string Code { get; set; } = null!;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
    }
}
