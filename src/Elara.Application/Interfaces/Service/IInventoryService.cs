using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Inventory;

namespace Elara.Application.Interfaces.Service
{
    public interface IInventoryService
    {
        Task<PaginatedResponse<InventoryProductDto>> GetSellerInventoryAsync(long userId, GetInventoryRequest request);
        Task<PaginatedResponse<InventoryProductDto>> GetLowStockProductsAsync(long userId, int threshold = 10, PaginationRequest? request = null);
        Task<ProductStockDto> GetProductStockAsync(long userId, long productId);
        Task<ProductStockDto> UpdateProductStockAsync(long userId, long productId, int stockQuantity);
    }
}
