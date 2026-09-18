using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.SellerApplication;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface ISellerApplicationRepository
    {
        Task<PaginationQueryResult<SellerApplication>> GetApplicationsAsync(GetSellerApplicationsRequest request);

        Task<SellerApplication?> GetApplicationByIdAsync(long applicationId);
        Task<SellerApplication?> GetLatestApplicationByUserIdAsync(long userId);
        Task<SellerApplication?> GetPendingApplicationByUserIdAsync(long userId);

        Task<bool> HasPendingApplicationAsync(long userId);
        Task AddAsync(SellerApplication application);

        Task UpdateAsync(SellerApplication application);
    }
}
