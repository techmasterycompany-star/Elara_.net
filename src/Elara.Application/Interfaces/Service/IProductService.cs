using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Product;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Service
{
    public interface IProductService
    {
        // Admin Product Management
        Task<PaginatedResponse<ProductListDto>> GetAdminProductsAsync(ProductQuery query);

        Task<ProductDetailsDto> GetAdminProductByIdAsync(long productId);

        Task<ProductDetailsDto> CreateAdminProductAsync(AdminCreateProductDto dto);

        Task<ProductDetailsDto> UpdateAdminProductAsync(long productId, AdminUpdateProductDto dto);

        Task UpdateAdminProductStatusAsync(long productId, bool isActive);

        Task DeleteAdminProductAsync(long productId);

        // Admin Product Images Management
        Task<List<ProductImageDto>> AddAdminImagesAsync(long productId, IEnumerable<ImageUploadRequest> images);
        Task ReorderAdminImagesAsync(long productId, List<ImageDisplayOrderDto> images);
        Task UpdateAdminImageDisplayOrderAsync(long productId, long imageId, int displayOrder);
        Task DeleteAdminImageAsync(long productId, long imageId);

        // Seller Product Management
        Task<PaginatedResponse<ProductListDto>> GetSellerProductsAsync(long userId, ProductQuery query);

        Task<ProductDetailsDto> GetSellerProductByIdAsync(long userId, long productId);

        Task<ProductDetailsDto> CreateSellerProductAsync(long userId, SellerCreateProductDto dto);

        Task<ProductDetailsDto> UpdateSellerProductAsync(long userId, long productId, SellerUpdateProductDto dto);

        Task UpdateSellerProductStatusAsync(long userId, long productId, bool isActive);

        Task DeleteSellerProductAsync(long userId, long productId);

        // Seller Product Images Management
        Task<List<ProductImageDto>> AddSellerImagesAsync(long userId, long productId, IEnumerable<ImageUploadRequest> images);
        Task ReorderSellerImagesAsync(long userId, long productId, List<ImageDisplayOrderDto> images);
        Task UpdateSellerImageDisplayOrderAsync(long userId, long productId, long imageId, int displayOrder);
        Task DeleteSellerImageAsync(long userId, long productId, long imageId);


    }
}
