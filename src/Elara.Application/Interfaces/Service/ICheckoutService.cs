using Elara.Application.DTOs.Checkout;

namespace Elara.Application.Interfaces.Service
{
    public interface ICheckoutService
    {
        Task<IEnumerable<ShippingMethodDto>> GetShippingMethodsAsync();
        Task<CheckoutPreviewResponse> PreviewCheckoutAsync(long? userId, string? guestSessionId, CheckoutPreviewRequest request);
        Task<CheckoutResponse> ProcessCheckoutAsync(long? userId, string? guestSessionId, CheckoutRequest request);
    }
}
