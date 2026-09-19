using Elara.Domain.Enums;

namespace Elara.Application.DTOs.Checkout
{
    public class CheckoutPreviewRequest
    {
        public long? CartId { get; set; }
        public long ShippingMethodId { get; set; }
        public string? PromoCode { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; } = null!;
    }
}
