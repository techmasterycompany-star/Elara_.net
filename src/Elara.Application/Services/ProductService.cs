using AutoMapper;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Product;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;
using Elara.Domain.Enums;

namespace Elara.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, ISellerRepository sellerProfileRepository, IStorageService cloudinaryService, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _sellerRepository = sellerProfileRepository;
            _storageService = cloudinaryService;
            _mapper = mapper;
        }

        // Admin

        public async Task<PaginatedResponse<ProductListDto>> GetAdminProductsAsync(ProductQuery query)
        {
            var result = await _productRepository.GetProductsAsync(query);

            return CreatePaginatedResponse<ProductListDto>(result, query);
        }

        public async Task<ProductDetailsDto> GetAdminProductByIdAsync(long productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new NotFoundException("Product not found.");

            return _mapper.Map<ProductDetailsDto>(product);
        }

        public async Task<ProductDetailsDto> CreateAdminProductAsync(AdminCreateProductDto dto)
        {
            await ValidateCategoryAsync(dto.CategoryId);
            await ValidateSellerAsync(dto.SellerProfileId);

            var product = _mapper.Map<Product>(dto);

            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            var createdProduct = await _productRepository.GetByIdAsync(product.Id);

            if (createdProduct == null)
                throw new NotFoundException("Product could not be retrieved after creation.");

            return _mapper.Map<ProductDetailsDto>(createdProduct);
        }

        public async Task<ProductDetailsDto> UpdateAdminProductAsync(long productId, AdminUpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new NotFoundException("Product not found.");

            await ValidateCategoryAsync(dto.CategoryId);
            await ValidateSellerAsync(dto.SellerProfileId);

            _mapper.Map(dto, product);

            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            return _mapper.Map<ProductDetailsDto>(product);
        }

        public async Task UpdateAdminProductStatusAsync(long productId, bool isActive)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new NotFoundException("Product not found.");

            product.IsActive = isActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task DeleteAdminProductAsync(long productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new NotFoundException("Product not found.");

            foreach (var image in product.Images)
                await _storageService.DeleteAsync(image.ImagePublicId);

            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        // SELLER

        public async Task<PaginatedResponse<ProductListDto>> GetSellerProductsAsync(long userId, ProductQuery query)
        {
            var seller = await GetSellerProfileByUserIdAsync(userId);

            var result = await _productRepository.GetProductsAsync(query, seller.Id);

            return CreatePaginatedResponse<ProductListDto>(result, query);
        }

        public async Task<ProductDetailsDto> GetSellerProductByIdAsync(long userId, long productId)
        {
            var seller = await GetSellerProfileByUserIdAsync(userId);

            var product = await _productRepository.GetByIdAsync(productId, seller.Id);

            if (product == null)
                throw new NotFoundException("Product not found.");

            return _mapper.Map<ProductDetailsDto>(product);
        }

        public async Task<ProductDetailsDto> CreateSellerProductAsync(long userId, SellerCreateProductDto dto)
        {
            var seller = await GetSellerProfileByUserIdAsync(userId);

            await ValidateCategoryAsync(dto.CategoryId);

            var product = _mapper.Map<Product>(dto);

            product.SellerProfileId = seller.Id;
            product.IsActive = false;
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            var createdProduct = await _productRepository.GetByIdAsync(product.Id);

            if (createdProduct == null)
                throw new NotFoundException("Product could not be retrieved after creation.");

            return _mapper.Map<ProductDetailsDto>(createdProduct);
        }

        public async Task<ProductDetailsDto> UpdateSellerProductAsync(long userId, long productId, SellerUpdateProductDto dto)
        {
            var seller = await GetSellerProfileByUserIdAsync(userId);

            var product = await _productRepository.GetByIdAsync(productId, seller.Id);

            if (product == null)
                throw new NotFoundException("Product not found.");

            await ValidateCategoryAsync(dto.CategoryId);

            _mapper.Map(dto, product);

            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            return _mapper.Map<ProductDetailsDto>(product);
        }

        public async Task UpdateSellerProductStatusAsync(long userId, long productId, bool isActive)
        {
            var seller = await GetSellerProfileByUserIdAsync(userId);

            var product = await _productRepository.GetByIdAsync(productId, seller.Id);

            if (product == null)
                throw new NotFoundException("Product not found.");

            product.IsActive = isActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task DeleteSellerProductAsync(long userId, long productId)
        {
            var seller = await GetSellerProfileByUserIdAsync(userId);

            var product = await _productRepository.GetByIdAsync(productId, seller.Id);

            if (product == null)
                throw new NotFoundException("Product not found.");

            foreach (var image in product.Images)
                await _storageService.DeleteAsync(image.ImagePublicId);

            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        // ADMIN IMAGES

        public async Task<List<ProductImageDto>> AddAdminImagesAsync(long productId, IEnumerable<ImageUploadRequest> images)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new NotFoundException("Product not found.");

            var uploadedImages = await UploadImagesAsync(product, images);

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            return _mapper.Map<List<ProductImageDto>>(uploadedImages);
        }

        public async Task UpdateAdminImageDisplayOrderAsync(long productId, long imageId, int displayOrder)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) throw new NotFoundException("Product not found.");

            await UpdateImageDisplayOrderAsync(product, imageId, displayOrder);
        }

        public async Task ReorderAdminImagesAsync(long productId, List<ImageDisplayOrderDto> images)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new NotFoundException("Product not found.");

            ApplyImageOrder(product, images);

            await _productRepository.SaveChangesAsync();
        }

        public async Task DeleteAdminImageAsync(long productId, long imageId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new NotFoundException("Product not found.");

            await DeleteProductImageAsync(product, imageId);
        }

        // SELLER IMAGES

        public async Task<List<ProductImageDto>> AddSellerImagesAsync(long userId, long productId, IEnumerable<ImageUploadRequest> images)
        {
            var product = await GetSellerProductAsync(userId, productId);

            var uploadedImages = await UploadImagesAsync(product, images);

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            return _mapper.Map<List<ProductImageDto>>(uploadedImages);
        }

        public async Task UpdateSellerImageDisplayOrderAsync(long userId, long productId, long imageId, int displayOrder)
        {
            var product = await GetSellerProductAsync(userId, productId);

            await UpdateImageDisplayOrderAsync(product, imageId, displayOrder);
        }

        public async Task ReorderSellerImagesAsync(long userId, long productId, List<ImageDisplayOrderDto> images)
        {
            var product = await GetSellerProductAsync(userId, productId);

            ApplyImageOrder(product, images);

            await _productRepository.SaveChangesAsync();
        }

        public async Task DeleteSellerImageAsync(long userId, long productId, long imageId)
        {
            var product = await GetSellerProductAsync(userId, productId);

            await DeleteProductImageAsync(product, imageId);
        }

        // HELPERS

        private async Task<Category> ValidateCategoryAsync(long categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);

            if (category == null || category.IsDeleted)
                throw new NotFoundException("Category not found.");

            return category;
        }

        private async Task<SellerProfile> ValidateSellerAsync(long sellerProfileId)
        {
            var seller = await _sellerRepository.GetByIdAsync(sellerProfileId);

            if (seller == null)
                throw new NotFoundException("Seller profile not found.");

            if (seller.IsApproved != true)
                throw new BadRequestException("Seller is not approved.");

            return seller;
        }

        private async Task<SellerProfile> GetSellerProfileByUserIdAsync(long userId)
        {
            var seller = await _sellerRepository.GetByUserIdAsync(userId);

            if (seller == null)
                throw new NotFoundException("Seller profile not found.");

            if (seller.IsApproved != true)
                throw new BadRequestException("Seller is not approved.");

            return seller;
        }

        private async Task<Product> GetSellerProductAsync(long userId, long productId)
        {
            var seller = await GetSellerProfileByUserIdAsync(userId);
            var product = await _productRepository.GetByIdAsync(productId, seller.Id);

            if (product == null)
                throw new NotFoundException("Product not found.");

            return product;
        }

        private async Task<List<ProductImage>> UploadImagesAsync(Product product, IEnumerable<ImageUploadRequest> images)
        {
            var imageList = images.ToList();

            if (!imageList.Any())
                throw new BadRequestException("At least one image is required.");

            if (product.Images.Count + imageList.Count > 10)
                throw new BadRequestException("A product can have a maximum of 10 images.");

            var nextDisplayOrder = product.Images.Any() ? product.Images.Max(x => x.DisplayOrder) + 1 : 1;
            var uploadedImages = new List<ProductImage>();

            try
            {
                foreach (var image in imageList)
                {
                    ValidateImage(image);

                    var result = await _storageService.UploadAsync(image.Stream, image.FileName);

                    var productImage = new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = result.Url,
                        ImagePublicId = result.PublicId,
                        DisplayOrder = nextDisplayOrder++,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    product.Images.Add(productImage);
                    uploadedImages.Add(productImage);
                }

                return uploadedImages;
            }
            catch
            {
                foreach (var image in uploadedImages)
                    await _storageService.DeleteAsync(image.ImagePublicId);

                throw;
            }
        }

        private async Task DeleteProductImageAsync(Product product, long imageId)
        {
            var image = GetProductImage(product, imageId);

            await _storageService.DeleteAsync(image.ImagePublicId);

            product.Images.Remove(image);

            await _productRepository.SaveChangesAsync();
        }

        private static ProductImage GetProductImage(Product product, long imageId)
        {
            var image = product.Images.FirstOrDefault(x => x.Id == imageId);

            if (image == null)
                throw new NotFoundException("Image not found.");

            return image;
        }

        private static void ApplyImageOrder(Product product, List<ImageDisplayOrderDto> images)
        {
            if (!images.Any())
                throw new BadRequestException("Images are required.");

            if(product.Images.Count == 0)
                throw new BadRequestException("The product has no images to reorder.");

            var productImageIds = product.Images.Select(x => x.Id).ToHashSet();
            var requestedImageIds = images.Select(x => x.ImageId).ToHashSet();


            if (!productImageIds.SetEquals(requestedImageIds))
                throw new BadRequestException("All product images must be included when reordering.");

            if (images.Any(x => x.DisplayOrder < 1))
                throw new BadRequestException("Display order must be greater than zero.");

            if (images.Select(x => x.DisplayOrder).Distinct().Count() != images.Count)
                throw new BadRequestException("Display orders must be unique.");

            foreach (var item in images)
            {
                var image = product.Images.First(x => x.Id == item.ImageId);
                image.DisplayOrder = item.DisplayOrder;
                image.UpdatedAt = DateTime.UtcNow;
            }
        }
        
        private async Task UpdateImageDisplayOrderAsync(Product product, long imageId, int newDisplayOrder)
        {
            var image = GetProductImage(product, imageId);
            var images = product.Images.OrderBy(x => x.DisplayOrder).ToList();

            if (newDisplayOrder < 1 || newDisplayOrder > images.Count)
                throw new BadRequestException($"Display order must be between 1 and {images.Count}.");

            var oldDisplayOrder = image.DisplayOrder;

            if (oldDisplayOrder == newDisplayOrder)
                return;

            if (newDisplayOrder < oldDisplayOrder)
            {
                foreach (var item in images.Where(x => x.DisplayOrder >= newDisplayOrder && x.DisplayOrder < oldDisplayOrder))
                {
                    item.DisplayOrder++;
                    item.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                foreach (var item in images.Where(x => x.DisplayOrder > oldDisplayOrder && x.DisplayOrder <= newDisplayOrder))
                {
                    item.DisplayOrder--;
                    item.UpdatedAt = DateTime.UtcNow;
                }
            }

            image.DisplayOrder = newDisplayOrder;
            image.UpdatedAt = DateTime.UtcNow;

            await _productRepository.SaveChangesAsync();
        }
        private static void ValidateImage(ImageUploadRequest image)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            const long maxSize = 5 * 1024 * 1024;

            if (!allowedTypes.Contains(image.ContentType))
                throw new BadRequestException("Only JPEG, PNG and WebP images are allowed.");

            if (image.Length > maxSize)
                throw new BadRequestException("Image size cannot exceed 5 MB.");
        }

        private PaginatedResponse<TDto> CreatePaginatedResponse<TDto>(PaginationQueryResult<Product> result, ProductQuery query)
        {
            return new PaginatedResponse<TDto>
            {
                Data = _mapper.Map<List<TDto>>(result.Items),
                PageNumber = query.PageNumber,
                Limit = query.Limit,
                TotalCount = result.TotalCount,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)query.Limit)
            };
        }
    }
}

