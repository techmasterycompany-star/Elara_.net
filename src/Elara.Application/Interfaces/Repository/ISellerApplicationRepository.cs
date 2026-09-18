using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.SellerApplication;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface ISellerApplicationRepository
    {
        Task<PaginationQueryResult<SellerApplication>> GetApplicationsAsync(GetSellerApplicationsRequest request);

        Task<SellerApplication?> GetApplicationByIdAsync(long applicationId);

        Task<bool> HasPendingApplicationAsync(long userId);

        Task UpdateAsync(SellerApplication application);
    }
}
