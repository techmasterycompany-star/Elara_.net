using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.SellerProfile;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface ISellerRepository
    {
        Task<SellerProfile?> GetByUserIdAsync(long userId);

        Task<SellerProfile?> GetByIdAsync(long sellerId, bool forPublic = false);

        Task<PaginationQueryResult<Product>> GetSellerProductsAsync( long sellerId, GetSellerProductsRequest request);

        Task UpdateProfileAsync(SellerProfile sellerProfile);

        Task<PaginationQueryResult<SellerProfile>> GetSellersAsync(GetSellersRequest request);
    }
}
