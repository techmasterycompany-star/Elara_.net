using Elara.Application.DTOs.Cart;
using Elara.Application.DTOs.Common;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using Elara.API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Elara.API.Controllers.Customer
{
    [Route("api/v1/customers/me/cart")]
    [ApiController]
    [Authorize(Roles="Customer")]
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
            long userId = User.GetAuthenticatedUserId();

            var cart = await _cartService.GetCartItemsAsync(userId);
            var response = ApiResponse<CartDto>.SuccessResponse(cart);
            return Ok(response);
        }

        // Add a product to the customer's cart
        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            long userId = User.GetAuthenticatedUserId();

            var cartItem = await _cartService.AddToCartAsync(userId, addToCartDto);
            var response = ApiResponse<CartItemDto>.SuccessResponse(cartItem);
            return Ok(response);
        }

        // Update the quantity of a cart item
        [HttpPatch("items/{productId}/quantity")]
        public async Task<IActionResult> UpdateCartItemQuantity(long productId, [FromBody] UpdateCartItemQuantityDto updateDto)
        {
            long userId = User.GetAuthenticatedUserId();

            var cartItem = await _cartService.UpdateCartItemQuantityAsync(userId, productId, updateDto.Quantity);
            var response = ApiResponse<CartItemDto>.SuccessResponse(cartItem);
            return Ok(response);
        }
       
        // Remove a product from the customer's cart
        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveFromCart(long productId)
        {
            long userId = User.GetAuthenticatedUserId();

            await _cartService.RemoveFromCartAsync(userId, productId);
            return NoContent();
        }
    }
}
