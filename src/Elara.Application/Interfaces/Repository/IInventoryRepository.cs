using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Inventory;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<Product>> GetSellerInventoryAsync(long sellerProfileId, GetInventoryRequest request);
        Task<int> GetSellerInventoryCountAsync(long sellerProfileId, GetInventoryRequest request);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(long sellerProfileId, int threshold, PaginationRequest? request = null);
        Task<int> GetLowStockProductsCountAsync(long sellerProfileId, int threshold);
        Task<Product?> GetProductStockAsync(long productId, long sellerProfileId);
        Task<bool> UpdateProductStockAsync(long productId, long sellerProfileId, int stockQuantity);
        Task<SellerProfile?> GetSellerProfileByUserIdAsync(long userId);
    }
}
