using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(long userId);
        Task<Cart> CreateCartAsync(long userId);
        Task<CartItem?> GetCartItemAsync(long cartId, long productId);
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task UpdateCartItemAsync(CartItem cartItem);
        Task RemoveCartItemAsync(long cartItemId);
        Task<Product?> GetProductByIdAsync(long productId);
    }
}
