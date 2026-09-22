using Elara.Domain.Enums;
using Elara.Application.DTOs.Payment;

namespace Elara.Application.DTOs.Checkout
{
    public class CheckoutRequest
    {
        public long? CartId { get; set; }
        public long ShippingMethodId { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; } = null!;
        public PaymentMethodType PaymentMethod { get; set; }
        public string? PromoCode { get; set; }
        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
        public string? PayPalEmail { get; set; }
        public CardDetailsDto? CardDetails { get; set; }
    }
}
