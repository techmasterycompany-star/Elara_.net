using AutoMapper;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.SellerApplication;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Application.Services
{
    public class SellerApplicationService : ISellerApplicationService
    {
        private readonly ISellerApplicationRepository _sellerApplicationRepository;
        private readonly IMapper _mapper;

        public SellerApplicationService(ISellerApplicationRepository sellerApplicationRepository, IMapper mapper)
        {
            _sellerApplicationRepository = sellerApplicationRepository;
            _mapper = mapper;
        }

        public async Task<SellerApplicationDetailsDto> GetApplicationByIdAsync(long applicationId)
        {
            var application = await _sellerApplicationRepository.GetApplicationByIdAsync(applicationId);

            if (application == null)
                throw new NotFoundException("Seller application not found.");

            return _mapper.Map<SellerApplicationDetailsDto>(application);
        }

        public async Task<PaginatedResponse<SellerApplicationListDto>> GetApplicationsAsync(GetSellerApplicationsRequest request)
        {
            var result = await _sellerApplicationRepository.GetApplicationsAsync(request);
            var MappedResult = _mapper.Map<List<SellerApplicationListDto>>(result.Items);

            return new PaginatedResponse<SellerApplicationListDto>
            {
                Data = MappedResult,
                PageNumber = request.PageNumber,
                Limit = request.Limit,
                TotalCount = result.TotalCount,
                TotalPages = (int)Math.Ceiling((double)result.TotalCount / request.Limit)
            };
        }
        public async Task ApproveApplicationAsync(long applicationId)
        {
            var application = await _sellerApplicationRepository.GetApplicationByIdAsync(applicationId);

            if (application == null)
                throw new NotFoundException("Seller application not found.");

            if (application.Status != SellerApplicationStatus.Pending)
                throw new ConflictException($"Cannot update an application with status {application.Status}.");

            application.Status = SellerApplicationStatus.Approved;
            application.RejectionReason = null;
            application.UpdatedAt = DateTime.UtcNow;

            var sellerProfile = application.User.SellerProfile;

            if (sellerProfile == null)
            {
                sellerProfile = new SellerProfile
                {
                    UserId = application.UserId,
                    StoreName = application.StoreName,
                    StoreDescription = application.StoreDescription,
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                application.User.SellerProfile = sellerProfile;
                application.User.UserRoles.Add(new UserRole { RoleId = 2, UserId = application.UserId });
            }
            else
            {
                sellerProfile.StoreName = application.StoreName;
                sellerProfile.StoreDescription = application.StoreDescription;
                sellerProfile.IsApproved = true;
                sellerProfile.UpdatedAt = DateTime.UtcNow;
            }

            await _sellerApplicationRepository.UpdateAsync(application);
        }

        public async Task RejectApplicationAsync(long applicationId, RejectSellerApplicationDto rejectDto)
        {
            var application = await _sellerApplicationRepository.GetApplicationByIdAsync(applicationId);

            if (application == null)
                throw new NotFoundException("Seller application not found.");

            if (application.Status != SellerApplicationStatus.Pending)
                throw new ConflictException($"Cannot update an application with status {application.Status}.");

            if (string.IsNullOrWhiteSpace(rejectDto.RejectionReason))
            {
                throw new ValidationException("Rejection reason is required when rejecting an application.");
            }

            application.Status = SellerApplicationStatus.Rejected;
            application.RejectionReason = rejectDto.RejectionReason;
            application.UpdatedAt = DateTime.UtcNow;         

            await _sellerApplicationRepository.UpdateAsync(application);
        }
    }
}
