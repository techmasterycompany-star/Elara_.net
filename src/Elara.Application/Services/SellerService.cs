using AutoMapper;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.SellerApplication;
using Elara.Application.DTOs.SellerProfile;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;
using Elara.Domain.Enums;

namespace Elara.Application.Services
{
    internal class SellerService : ISellerService
    {
        private readonly ISellerApplicationRepository _sellerApplicationRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly IMapper _mapper;

        public SellerService(ISellerApplicationRepository sellerApplicationRepository, ISellerRepository sellerRepository, IMapper mapper)
        {
            _sellerApplicationRepository = sellerApplicationRepository;
            _sellerRepository = sellerRepository;
            _mapper = mapper;
        }

        public async Task ApplyAsync(long userId, ApplyAsSellerDto request)
        {
            var existingApplication = await _sellerApplicationRepository.GetLatestApplicationByUserIdAsync(userId);
            var existingSeller = await _sellerRepository.GetByUserIdAsync(userId);

            if (existingApplication != null)
            {
                if (existingApplication.Status == SellerApplicationStatus.Pending)
                    throw new ConflictException("You already have a pending seller application.");

                if (existingApplication.Status == SellerApplicationStatus.Approved)
                    throw new ConflictException("You are already an approved seller.");
            }

            if (existingSeller != null)
                throw new ConflictException("You are already an approved seller.");

            var application = new SellerApplication
            {
                UserId = userId,
                StoreName = request.StoreName,
                StoreDescription = request.StoreDescription,
                Status = SellerApplicationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _sellerApplicationRepository.AddAsync(application);
        }

        public async Task<SellerApplicationDto> GetMyApplicationAsync(long userId)
        {
            var application = await _sellerApplicationRepository.GetLatestApplicationByUserIdAsync(userId);

            if (application == null)
                throw new NotFoundException("Seller application not found.");

            return _mapper.Map<SellerApplicationDto>(application);
        }

        public async Task<SellerProfileDto> GetMyProfileAsync(long userId)
        {
            var seller = await _sellerRepository.GetByUserIdAsync(userId);

            if (seller == null)
                throw new NotFoundException("Seller profile not found.");

            return _mapper.Map<SellerProfileDto>(seller);
        }

        public async Task<PaginatedResponse<SellerListDto>> GetSellersAsync(GetSellersRequest request)
        {
            var sellers = await _sellerRepository.GetSellersAsync(request);
            var mappedSellers = _mapper.Map<IEnumerable<SellerListDto>>(sellers.Items);

            return new PaginatedResponse<SellerListDto>
            {
                Data = mappedSellers,
                PageNumber = request.PageNumber,
                Limit = request.Limit,
                TotalCount = sellers.TotalCount,
                TotalPages = (int)Math.Ceiling((double)sellers.TotalCount / request.Limit)
            };
        }

        public async Task<SellerListDto> GetSellerByIdAsync(long sellerId)
        {
            var seller = await _sellerRepository.GetByIdAsync(sellerId, forPublic: true);

            if (seller == null)
                throw new NotFoundException("Seller not found.");

            return _mapper.Map<SellerListDto>(seller);
        }

        public async Task<PaginatedResponse<SellerProductDto>> GetSellerProductsAsync(long sellerId, GetSellerProductsRequest request)
        {
            var seller = await _sellerRepository.GetByIdAsync(sellerId, forPublic:true);

            if (seller == null)
                throw new NotFoundException("Seller not found.");

            var result = await _sellerRepository
                .GetSellerProductsAsync(sellerId, request);

            return new PaginatedResponse<SellerProductDto>
            {
                Data = _mapper.Map<List<SellerProductDto>>(result.Items),
                PageNumber = request.PageNumber,
                Limit = request.Limit,
                TotalCount = result.TotalCount,
                TotalPages = (int)Math.Ceiling((double)result.TotalCount / request.Limit)
            };
        }

        public async Task UpdateMyProfileAsync(long userId, UpdateSellerProfileDto request)
        {
            var seller = await _sellerRepository.GetByUserIdAsync(userId);

            if (seller == null)
                throw new NotFoundException("Seller profile not found.");

            if (!seller.IsApproved)
                throw new ConflictException("Seller profile is not approved.");

            _mapper.Map(request, seller);

            seller.UpdatedAt = DateTime.UtcNow;

            await _sellerRepository.UpdateProfileAsync(seller);
        }

        public async Task WithdrawApplicationAsync(long userId)
        {
            var application = await _sellerApplicationRepository.GetPendingApplicationByUserIdAsync(userId);

            if (application == null)
                throw new NotFoundException("No pending seller application found.");

            application.Status = SellerApplicationStatus.Withdrawn;
            application.UpdatedAt = DateTime.UtcNow;

            await _sellerApplicationRepository.UpdateAsync(application);
        }
    }
}
