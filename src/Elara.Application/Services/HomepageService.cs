using AutoMapper;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.HomePageContent;
using Elara.Application.DTOs.Product;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;
using Elara.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Elara.Application.Services
{
    public class HomepageService : IHomepageService
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly IHomepageSectionRepository _sectionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public HomepageService(IBannerRepository bannerRepository, IHomepageSectionRepository sectionRepository, IStorageService cloudinaryService, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _bannerRepository = bannerRepository;
            _sectionRepository = sectionRepository;
            _storageService = cloudinaryService;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<HomepageDto> GetHomepageAsync()
        {
            var banners = await GetActiveBannersAsync();
            var sections = await GetActiveSectionsAsync();

            return new HomepageDto
            {
                Banners = banners,
                Sections = sections
            };
        }

        public async Task<IEnumerable<BannerDto>> GetActiveBannersAsync()
        {
            var banners = await _bannerRepository.GetActiveAsync();

            return _mapper.Map<IEnumerable<BannerDto>>(banners);
        }

        public async Task<IEnumerable<HomepageSectionDto>> GetActiveSectionsAsync()
        {
            var sections = await _sectionRepository.GetActiveAsync();

            return _mapper.Map<IEnumerable<HomepageSectionDto>>(sections);
        }

        public async Task<PaginatedResponse<BannerDto>> GetAdminBannersAsync(AdminBannerQuery query)
        {
            var result = await _bannerRepository.GetAdminBannersAsync(query);

            var totalPages = (int)Math.Ceiling((double)result.TotalCount / query.Limit);

            return new PaginatedResponse<BannerDto>
            {
                Data = _mapper.Map<IEnumerable<BannerDto>>(result.Items),
                PageNumber = query.PageNumber,
                Limit = query.Limit,
                TotalCount = result.TotalCount,
                TotalPages = totalPages
            };
        }

        public async Task<PaginatedResponse<HomepageSectionDto>> GetAdminSectionsAsync(HomepageSectionQuery query)
        {
            var result = await _sectionRepository.GetSectionsAsync(query);
            var data = _mapper.Map<IEnumerable<HomepageSectionDto>>(result.Items);

            return new PaginatedResponse<HomepageSectionDto>
            {
                Data = data,
                PageNumber = query.PageNumber,
                Limit = query.Limit,
                TotalCount = result.TotalCount,
                TotalPages = (int)Math.Ceiling((double)result.TotalCount / query.Limit)
            };
        }

        public async Task<BannerDto> CreateBannerAsync(CreateBannerDto dto)
        {
            ValidateBannerDates(dto.StartDate, dto.EndDate);

            var upload = await UploadBannerImageAsync(dto.Image);

            var banner = new Banner
            {
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                ImageUrl = upload.Url,
                ImagePublicId = upload.PublicId,
                LinkUrl = dto.LinkUrl,
                Position = dto.Position,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _bannerRepository.AddAsync(banner);
            await _bannerRepository.SaveChangesAsync();

            return _mapper.Map<BannerDto>(banner);
        }

        public async Task<BannerDto> UpdateBannerAsync(long bannerId, UpdateBannerDto dto)
        {
            var banner = await GetBannerAsync(bannerId);

            ValidateBannerDates(dto.StartDate, dto.EndDate);


            string? oldPublicId = null;

            if (dto.Image != null)
            {
                oldPublicId = banner.ImagePublicId;

                var upload = await UploadBannerImageAsync(dto.Image);

                banner.ImageUrl = upload.Url;
                banner.ImagePublicId = upload.PublicId;
            }

            banner.Title = dto.Title;
            banner.Subtitle = dto.Subtitle;
            banner.LinkUrl = dto.LinkUrl;
            banner.Position = dto.Position;
            banner.DisplayOrder = dto.DisplayOrder;
            banner.StartDate = dto.StartDate;
            banner.EndDate = dto.EndDate;
            banner.UpdatedAt = DateTime.UtcNow;

            await _bannerRepository.UpdateAsync(banner);
            await _bannerRepository.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(oldPublicId))
            {
                await _storageService.DeleteAsync(oldPublicId);
            }

            return _mapper.Map<BannerDto>(banner);
        }

        public async Task UpdateBannerStatusAsync(long bannerId, bool isActive)
        {
            var banner = await GetBannerAsync(bannerId);

            banner.IsActive = isActive;
            banner.UpdatedAt = DateTime.UtcNow;

            await _bannerRepository.UpdateAsync(banner);
            await _bannerRepository.SaveChangesAsync();
        }

        public async Task DeleteBannerAsync(long bannerId)
        {
            var banner = await GetBannerAsync(bannerId);

            var publicId = banner.ImagePublicId;

            await _bannerRepository.DeleteAsync(banner);
            await _bannerRepository.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(publicId))
                await _storageService.DeleteAsync(publicId);
        }

        public async Task<HomepageSectionDto> CreateSectionAsync(CreateHomepageSectionDto dto)
        {
            ValidateSection(dto.Type, dto.BannerId, dto.CategoryId, dto.MaxItems);
            await ValidateSectionReferencesAsync(dto.Type, dto.BannerId, dto.CategoryId);

            var section = _mapper.Map<HomepageSection>(dto);
            section.CreatedAt = DateTime.UtcNow;
            section.UpdatedAt = DateTime.UtcNow;

            await _sectionRepository.AddAsync(section);
            await _sectionRepository.SaveChangesAsync();

            return _mapper.Map<HomepageSectionDto>(section);
        }

        public async Task<HomepageSectionDto> UpdateSectionAsync(long sectionId, UpdateHomepageSectionDto dto)
        {
            var section = await GetSectionAsync(sectionId);

            ValidateSection(dto.Type, dto.BannerId, dto.CategoryId, dto.MaxItems);

            await ValidateSectionReferencesAsync(dto.Type, dto.BannerId, dto.CategoryId);

            _mapper.Map(dto, section);
            section.UpdatedAt = DateTime.UtcNow;

            await _sectionRepository.UpdateAsync(section);
            await _sectionRepository.SaveChangesAsync();

            return _mapper.Map<HomepageSectionDto>(section);
        }

        public async Task DeleteSectionAsync(long sectionId)
        {
            var section = await GetSectionAsync(sectionId);

            await _sectionRepository.DeleteAsync(section);
            await _sectionRepository.SaveChangesAsync();
        }

        private async Task<Banner> GetBannerAsync(long bannerId)
        {
            var banner = await _bannerRepository.GetByIdAsync(bannerId);

            if (banner == null)
                throw new NotFoundException("Banner not found.");

            return banner;
        }

        private async Task<HomepageSection> GetSectionAsync(long sectionId)
        {
            var section = await _sectionRepository.GetByIdAsync(sectionId);

            if (section == null)
                throw new NotFoundException("Homepage section not found.");

            return section;
        }

        private async Task<CloudinaryUploadResult> UploadBannerImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new BadRequestException("Banner image is required.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };

            if (!allowedTypes.Contains(image.ContentType))
                throw new BadRequestException("Only JPEG, PNG and WebP images are allowed.");

            if (image.Length > 5 * 1024 * 1024)
                throw new BadRequestException("Banner image cannot exceed 5 MB.");

            await using var stream = image.OpenReadStream();

            return await _storageService.UploadAsync(stream, image.FileName);
        }

        private static void ValidateBannerDates(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && endDate < startDate)
                throw new BadRequestException("End date cannot be earlier than start date.");

        }

        private static void ValidateSection(HomepageSectionType type, long? bannerId, long? categoryId, int? maxItems)
        {
            if (!Enum.IsDefined(typeof(HomepageSectionType), type))
                throw new BadRequestException("Invalid homepage section type.");

            if (type == HomepageSectionType.Banner)
            {
                if (!bannerId.HasValue)
                    throw new BadRequestException("BannerId is required for a Banner section.");

                if (categoryId.HasValue)
                    throw new BadRequestException("CategoryId cannot be used for a Banner section.");

                if (maxItems.HasValue)
                    throw new BadRequestException("MaxItems cannot be used for a Banner section.");

                return;
            }

            if (bannerId.HasValue)
                throw new BadRequestException("BannerId can only be used for a Banner section.");

            if (!maxItems.HasValue || maxItems.Value < 1)
                throw new BadRequestException("MaxItems must be greater than zero for this section type.");

            if (type == HomepageSectionType.CategoryProducts)
            {
                if (!categoryId.HasValue)
                    throw new BadRequestException("CategoryId is required for a CategoryProducts section.");

                return;
            }

            if (categoryId.HasValue)
                throw new BadRequestException("CategoryId can only be used for a CategoryProducts section.");
        }

        private async Task ValidateSectionReferencesAsync(HomepageSectionType type, long? bannerId, long? categoryId)
        {
            if (type == HomepageSectionType.Banner)
            {
                var banner = await _bannerRepository.GetByIdAsync(bannerId!.Value);

                if (banner == null)
                    throw new NotFoundException("Banner not found.");

                return;
            }

            if (type == HomepageSectionType.CategoryProducts)
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(categoryId!.Value);

                if (category == null || category.IsDeleted)
                    throw new NotFoundException("Category not found.");
            }
        }
    }
}
