using Elara.Application.DTOs.Cart;
using Elara.Application.DTOs.Common;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using Elara.API.Helpers;

namespace Elara.API.Controllers.Customer
{
    [Route("api/v1/customers/me/cart")]
    [ApiController]
    public class CustomerCartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CustomerCartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Get all items in the customer's cart
        [HttpGet("items")]
        public async Task<IActionResult> GetCartItems()
        {
            var cart = await _cartService.GetCartItemsAsync(GetUserId(), GetGuestSessionId());
            var response = ApiResponse<CartDto>.SuccessResponse(cart);
            return Ok(response);
        }

        // Add a product to the customer's cart
        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            var cartItem = await _cartService.AddToCartAsync(GetUserId(), GetGuestSessionId(), addToCartDto);
            var response = ApiResponse<CartItemDto>.SuccessResponse(cartItem);
            return Ok(response);
        }

        // Update the quantity of a cart item
        [HttpPatch("items/{productId}/quantity")]
        public async Task<IActionResult> UpdateCartItemQuantity(long productId, [FromBody] UpdateCartItemQuantityDto updateDto)
        {
            var cartItem = await _cartService.UpdateCartItemQuantityAsync(GetUserId(), GetGuestSessionId(), productId, updateDto.Quantity);
            var response = ApiResponse<CartItemDto>.SuccessResponse(cartItem);
            return Ok(response);
        }

        // Remove a product from the customer's cart
        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveFromCart(long productId)
        {
            await _cartService.RemoveFromCartAsync(GetUserId(), GetGuestSessionId(), productId);
            return NoContent();
        }

        private long? GetUserId() => User.Identity?.IsAuthenticated == true
            ? User.GetAuthenticatedUserId()
            : null;

        private string? GetGuestSessionId() => Request.Cookies["Guest-Session-Id"];
    }
}
