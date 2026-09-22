using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.SellerApplication;
using Elara.Application.DTOs.SellerProfile;

namespace Elara.Application.Interfaces.Service
{
    public interface ISellerService
    {
        Task ApplyAsync(long userId, ApplyAsSellerDto request);

        Task<SellerApplicationDto> GetMyApplicationAsync(long userId);

        Task WithdrawApplicationAsync(long userId);

        Task<SellerProfileDto> GetMyProfileAsync(long userId);

        Task UpdateMyProfileAsync(long userId,UpdateSellerProfileDto request);

        Task<PaginatedResponse<SellerListDto>> GetSellersAsync(GetSellersRequest request);
        Task<SellerListDto> GetSellerByIdAsync(long sellerId);
        Task<PaginatedResponse<SellerProductDto>> GetSellerProductsAsync(long sellerId, GetSellerProductsRequest request);
    }
}
