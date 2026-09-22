using Elara.Application.DTOs.Checkout;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface ICheckoutRepository
    {
        Task<IEnumerable<ShippingMethod>> GetActiveShippingMethodsAsync();
        Task<ShippingMethod?> GetShippingMethodByIdAsync(long shippingMethodId);
        Task<PromoCode?> GetPromoCodeByCodeAsync(string code);
        Task<Cart?> GetCartWithItemsAsync(long cartId, long? userId, string? guestSessionId);
        Task<Product?> GetProductWithSellerAsync(long productId);
        Task<IEnumerable<Product>> GetProductsWithSellerAsync(IEnumerable<long> productIds);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> GetOrderAsync(Order order);

        Task<bool> ClearCartAsync(long cartId);
        Task CreateTransactionAsync(Func<Task> action);
        Task UpdatePaymentTransactionIdAsync(long paymentId, string transactionId);
        Task UpdateStockAsync(IEnumerable<CheckoutItemPreviewDto> checkoutItems);
    }
}
