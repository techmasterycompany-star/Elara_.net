using Elara.Domain.Enums;

namespace Elara.Application.DTOs.Checkout
{
    public class CheckoutRequest
    {
        public long? CartId { get; set; }
        public long ShippingMethodId { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; } = null!;
        public PaymentMethodType PaymentMethod { get; set; }
        public string? PromoCode { get; set; }
    }
}
