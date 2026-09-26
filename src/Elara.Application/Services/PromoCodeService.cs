using AutoMapper;
using Elara.Application.DTOs;
using Elara.Application.DTOs.Common;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces;
using Elara.Domain.Entities;
using Elara.Domain.Enums;

namespace Elara.Application.Services
{
    public class PromoCodeService : IPromoCodeService
    {
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly IMapper _mapper;

        public PromoCodeService(IPromoCodeRepository promoCodeRepository, IMapper mapper)
        {
            _promoCodeRepository = promoCodeRepository;
            _mapper = mapper;
        }

        public async Task<PromoCodeValidationResultDto> ValidateAsync(
            string code,
            decimal orderSubTotal,
            CancellationToken cancellationToken = default)
        {
            var promoCode = await _promoCodeRepository.GetByCodeAsync(code, cancellationToken);

            if (promoCode == null)
                return Invalid("This promo code does not exist.");

            if (!promoCode.IsActive)
                return Invalid("This promo code is no longer active.");

            if (promoCode.ExpiryDate < DateTime.UtcNow)
                return Invalid("This promo code has expired.");

            if (promoCode.UsageLimit > 0 && promoCode.TimesUsed >= promoCode.UsageLimit)
                return Invalid("This promo code has reached its usage limit.");

            if (orderSubTotal < promoCode.MinOrderAmount)
                return Invalid($"A minimum order of {promoCode.MinOrderAmount:C} is required to use this code.");

            var discount = promoCode.DiscountType == DiscountType.Percentage
                ? orderSubTotal * (promoCode.DiscountValue / 100m)
                : promoCode.DiscountValue;

            discount = Math.Min(discount, orderSubTotal);

            return new PromoCodeValidationResultDto
            {
                IsValid = true,
                DiscountAmount = discount,
                FinalTotal = orderSubTotal - discount
            };
        }

        public async Task<PromoCodeDetailsDto?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            var promoCode = await _promoCodeRepository.GetByCodeAsync(code, cancellationToken);

            if (promoCode == null)
                return null;

            return new PromoCodeDetailsDto
            {
                Code = promoCode.Code,
                DiscountType = promoCode.DiscountType.ToString(),
                DiscountValue = promoCode.DiscountValue,
                MinOrderAmount = promoCode.MinOrderAmount,
                ExpiryDate = promoCode.ExpiryDate,
                IsActive = promoCode.IsActive
            };
        }

        private static PromoCodeValidationResultDto Invalid(string message) => new()
        {
            IsValid = false,
            ErrorMessage = message
        };

        public async Task<PaginatedResponse<PromoCodeDto>> GetAdminPromoCodesAsync(PromoCodeQuery query)
        {
            var result = await _promoCodeRepository.GetAdminPromoCodesAsync(query);

            return new PaginatedResponse<PromoCodeDto>
            {
                Data = _mapper.Map<List<PromoCodeDto>>(result.Items),
                PageNumber = query.PageNumber,
                Limit = query.Limit,
                TotalCount = result.TotalCount,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)query.Limit)
            };
        }

        public async Task<PromoCodeDto> GetAdminPromoCodeByIdAsync(long promoCodeId)
        {
            var promoCode = await _promoCodeRepository.GetByIdAsync(promoCodeId);

            if (promoCode == null)
                throw new NotFoundException("Promo code not found.");

            return _mapper.Map<PromoCodeDto>(promoCode);
        }

        public async Task<PromoCodeDto> CreatePromoCodeAsync(CreatePromoCodeDto dto)
        {
            ValidatePromoCode(dto.DiscountType, dto.DiscountValue, dto.MinOrderAmount, dto.ExpiryDate, dto.UsageLimit);

            var code = NormalizeCode(dto.Code);

            if (await _promoCodeRepository.CodeExistsAsync(code))
                throw new ConflictException($"Promo code '{code}' already exists.");

            var promoCode = _mapper.Map<PromoCode>(dto);

            promoCode.Code = code;
            promoCode.TimesUsed = 0;
            promoCode.IsActive = true;
            promoCode.CreatedAt = DateTime.UtcNow;
            promoCode.UpdatedAt = DateTime.UtcNow;

            await _promoCodeRepository.AddAsync(promoCode);
            await _promoCodeRepository.SaveChangesAsync();

            return _mapper.Map<PromoCodeDto>(promoCode);
        }

        public async Task<PromoCodeDto> UpdatePromoCodeAsync(long promoCodeId, UpdatePromoCodeDto dto)
        {
            var promoCode = await _promoCodeRepository.GetByIdAsync(promoCodeId);

            if (promoCode == null)
                throw new NotFoundException("Promo code not found.");

            ValidatePromoCode(dto.DiscountType, dto.DiscountValue, dto.MinOrderAmount, dto.ExpiryDate, dto.UsageLimit);

            var code = NormalizeCode(dto.Code);

            if (await _promoCodeRepository.CodeExistsAsync(code, promoCodeId))
                throw new ConflictException($"Promo code '{code}' already exists.");

            if (dto.UsageLimit < promoCode.TimesUsed)
                throw new BadRequestException($"Usage limit cannot be less than the number of times the promo code has already been used ({promoCode.TimesUsed}).");

            _mapper.Map(dto, promoCode);

            promoCode.Code = code;
            promoCode.UpdatedAt = DateTime.UtcNow;

            await _promoCodeRepository.UpdateAsync(promoCode);
            await _promoCodeRepository.SaveChangesAsync();

            return _mapper.Map<PromoCodeDto>(promoCode);
        }

        public async Task UpdatePromoCodeStatusAsync(long promoCodeId, bool isActive)
        {
            var promoCode = await _promoCodeRepository.GetByIdAsync(promoCodeId);

            if (promoCode == null)
                throw new NotFoundException("Promo code not found.");

            if (isActive && promoCode.ExpiryDate <= DateTime.UtcNow)
                throw new BadRequestException("An expired promo code cannot be activated.");

            if (isActive && promoCode.TimesUsed >= promoCode.UsageLimit)
                throw new BadRequestException("A promo code that has reached its usage limit cannot be activated.");

            promoCode.IsActive = isActive;
            promoCode.UpdatedAt = DateTime.UtcNow;

            await _promoCodeRepository.UpdateAsync(promoCode);
            await _promoCodeRepository.SaveChangesAsync();
        }

        public async Task DeletePromoCodeAsync(long promoCodeId)
        {
            var promoCode = await _promoCodeRepository.GetByIdAsync(promoCodeId);

            if (promoCode == null)
                throw new NotFoundException("Promo code not found.");

            promoCode.IsDeleted = true;
            promoCode.DeletedAt = DateTime.UtcNow;
            promoCode.UpdatedAt = DateTime.UtcNow;
            promoCode.IsActive = false;

            await _promoCodeRepository.UpdateAsync(promoCode);
            await _promoCodeRepository.SaveChangesAsync();
        }

        private static string NormalizeCode(string code)
        {
            return code.Trim().ToUpperInvariant();
        }

        private static void ValidatePromoCode(DiscountType discountType, decimal discountValue, decimal minOrderAmount, DateTime expiryDate, int usageLimit)
        {
            if (!Enum.IsDefined(discountType))
                throw new BadRequestException("Invalid discount type.");

            if (discountValue <= 0)
                throw new BadRequestException("Discount value must be greater than zero.");

            if (discountType == DiscountType.Percentage && discountValue > 100)
                throw new BadRequestException("Percentage discount cannot exceed 100%.");

            if (minOrderAmount < 0)
                throw new BadRequestException("Minimum order amount cannot be negative.");

            if (expiryDate <= DateTime.UtcNow)
                throw new BadRequestException("Expiry date must be in the future.");

            if (usageLimit <= 0)
                throw new BadRequestException("Usage limit must be greater than zero.");
        }
    }
}