using Elara.Application.DTOs.Cart;

namespace Elara.Application.Interfaces.Service
{
    public interface ICartService
    {
        Task<GuestSessionDto> CreateGuestSessionAsync();
        Task<CartDto> GetCartItemsAsync(long? userId, string? guestSessionId);
        Task<CartItemDto> AddToCartAsync(long? userId, string? guestSessionId, AddToCartDto addToCartDto);
        Task RemoveFromCartAsync(long? userId, string? guestSessionId, long productId);
        Task<CartItemDto> UpdateCartItemQuantityAsync(long? userId, string? guestSessionId, long productId, int quantity);
    }
}
