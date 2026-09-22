using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(long userId);
        Task<Cart?> GetCartByGuestSessionIdAsync(string guestSessionId);
        Task<Cart> CreateCartAsync(long userId);
        Task<Cart> CreateGuestCartAsync(string guestSessionId);
        Task<CartItem?> GetCartItemAsync(long cartId, long productId);
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task UpdateCartItemAsync(CartItem cartItem);
        Task RemoveCartItemAsync(CartItem cartItem);
        Task<Product?> GetProductByIdAsync(long productId);
    }
}
