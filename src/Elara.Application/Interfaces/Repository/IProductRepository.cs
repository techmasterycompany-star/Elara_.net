using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Product;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IProductRepository
    {
        Task<PaginationQueryResult<Product>> GetProductsAsync(ProductQuery query, long? sellerProfileId = null);

        Task<Product?> GetByIdAsync(long productId, long? sellerProfileId = null , bool includeDeleted = false);

        Task<bool> ExistsAsync(long productId);

        Task<Product> AddAsync(Product product);

        Task UpdateAsync(Product product);

        Task SaveChangesAsync();
    }
}
