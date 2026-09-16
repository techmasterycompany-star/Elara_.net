namespace Elara.Application.DTOs.Order
{
    public class PaymentSummaryDto
    {
        public long Id { get; set; }

        // Adapt these properties to your actual Payment entity.
        public string? PaymentMethod { get; set; }
        public string? TransactionReference { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
    }

}
