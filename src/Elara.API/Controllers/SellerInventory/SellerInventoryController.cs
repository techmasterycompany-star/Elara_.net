using Elara.API.Helpers;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Inventory;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Seller
{
    [Route("api/v1/sellers/me")]
    [ApiController]
    public class SellerInventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public SellerInventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // Get seller inventory with pagination and filters
        [HttpGet("inventory")]
        public async Task<IActionResult> GetSellerInventory([FromQuery] GetInventoryRequest request)
        {
            var userId = User.GetAuthenticatedUserId();

            var inventory = await _inventoryService.GetSellerInventoryAsync(userId, request);
            var response = ApiResponse<PaginatedResponse<InventoryProductDto>>.SuccessResponse(inventory);
            return Ok(response);
        }

        // Get low-stock products for the seller with pagination
        [HttpGet("inventory/low-stock")]
        public async Task<IActionResult> GetLowStockProducts([FromQuery] int threshold = 10, [FromQuery] PaginationRequest? request = null)
        {
            var userId = User.GetAuthenticatedUserId();

            var lowStockProducts = await _inventoryService.GetLowStockProductsAsync(userId, threshold, request);
            var response = ApiResponse<PaginatedResponse<InventoryProductDto>>.SuccessResponse(lowStockProducts);
            return Ok(response);
        }

        // Get product stock information
        [HttpGet("products/{productId}/stock")]
        public async Task<IActionResult> GetProductStock(long productId)
        {
            var userId = User.GetAuthenticatedUserId();

            var productStock = await _inventoryService.GetProductStockAsync(userId, productId);
            var response = ApiResponse<ProductStockDto>.SuccessResponse(productStock);
            return Ok(response);
        }

        // Update product stock quantity
        [HttpPatch("products/{productId}/stock")]
        public async Task<IActionResult> UpdateProductStock(long productId, [FromBody] UpdateProductStockDto updateStockDto)
        {
            var userId = User.GetAuthenticatedUserId();

            var updatedProduct = await _inventoryService.UpdateProductStockAsync(userId, productId, updateStockDto.StockQuantity);
            var response = ApiResponse<ProductStockDto>.SuccessResponse(updatedProduct);
            return Ok(response);
        }
    }
}
