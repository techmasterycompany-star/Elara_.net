using Elara.Application.DTOs.Cart;

namespace Elara.Application.Interfaces.Service
{
    public interface ICartService
    {
        Task<CartDto> GetCartItemsAsync(long userId);
        Task<CartItemDto> AddToCartAsync(long userId, AddToCartDto addToCartDto);
        Task RemoveFromCartAsync(long userId, long productId);
        Task<CartItemDto> UpdateCartItemQuantityAsync(long userId, long productId, int quantity);
    }
}
