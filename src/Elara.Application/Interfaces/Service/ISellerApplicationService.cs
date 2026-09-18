using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.SellerApplication;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Service
{
    public interface ISellerApplicationService
    {
        Task<PaginatedResponse<SellerApplicationListDto>> GetApplicationsAsync(GetSellerApplicationsRequest request);

        Task<SellerApplicationDetailsDto> GetApplicationByIdAsync(long applicationId);

        Task ApproveApplicationAsync(long applicationId);
        Task RejectApplicationAsync(long applicationId, RejectSellerApplicationDto rejectDto);
    }
}
