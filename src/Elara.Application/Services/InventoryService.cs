using AutoMapper;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Inventory;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;

namespace Elara.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public InventoryService(IInventoryRepository inventoryRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<InventoryProductDto>> GetSellerInventoryAsync(long userId, GetInventoryRequest request)
        {
            var sellerProfile = await _inventoryRepository.GetSellerProfileByUserIdAsync(userId);
            if (sellerProfile == null)
                throw new NotFoundException("Seller profile not found");

            var products = await _inventoryRepository.GetSellerInventoryAsync(sellerProfile.Id, request);
            var totalCount = await _inventoryRepository.GetSellerInventoryCountAsync(sellerProfile.Id, request);

            var productDtos = _mapper.Map<IEnumerable<InventoryProductDto>>(products);

            return new PaginatedResponse<InventoryProductDto>
            {
                Data = productDtos,
                PageNumber = request.PageNumber,
                Limit = request.Limit,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.Limit)
            };
        }

        public async Task<PaginatedResponse<InventoryProductDto>> GetLowStockProductsAsync(long userId, int threshold = 10, PaginationRequest? request = null)
        {
            var sellerProfile = await _inventoryRepository.GetSellerProfileByUserIdAsync(userId);
            if (sellerProfile == null)
                throw new NotFoundException("Seller profile not found");

            // Use default pagination if not provided
            if (request == null)
            {
                request = new PaginationRequest
                {
                    PageNumber = 1,
                    Limit = 10
                };
            }

            var products = await _inventoryRepository.GetLowStockProductsAsync(sellerProfile.Id, threshold, request);
            var totalCount = await _inventoryRepository.GetLowStockProductsCountAsync(sellerProfile.Id, threshold);

            var productDtos = _mapper.Map<IEnumerable<InventoryProductDto>>(products);

            return new PaginatedResponse<InventoryProductDto>
            {
                Data = productDtos,
                PageNumber = request.PageNumber,
                Limit = request.Limit,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.Limit)
            };
        }

        public async Task<ProductStockDto> GetProductStockAsync(long userId, long productId)
        {
            var sellerProfile = await _inventoryRepository.GetSellerProfileByUserIdAsync(userId);
            if (sellerProfile == null)
                throw new NotFoundException("Seller profile not found");

            var product = await _inventoryRepository.GetProductStockAsync(productId, sellerProfile.Id);
            if (product == null)
                throw new NotFoundException("Product not found or does not belong to this seller");

            return _mapper.Map<ProductStockDto>(product);
        }

        public async Task<ProductStockDto> UpdateProductStockAsync(long userId, long productId, int stockQuantity)
        {
            // Validate stock quantity
            if (stockQuantity < 0)
                throw new BadRequestException("Stock quantity cannot be negative");

            var sellerProfile = await _inventoryRepository.GetSellerProfileByUserIdAsync(userId);
            if (sellerProfile == null)
                throw new NotFoundException("Seller profile not found");

            var product = await _inventoryRepository.GetProductStockAsync(productId, sellerProfile.Id);
            if (product == null)
                throw new NotFoundException("Product not found or does not belong to this seller");

            var updated = await _inventoryRepository.UpdateProductStockAsync(productId, sellerProfile.Id, stockQuantity);
            if (!updated)
                throw new BadRequestException("Failed to update product stock");

            // Fetch updated product
            var updatedProduct = await _inventoryRepository.GetProductStockAsync(productId, sellerProfile.Id);
            return _mapper.Map<ProductStockDto>(updatedProduct);
        }
    }
}
