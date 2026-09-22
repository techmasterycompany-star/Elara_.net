using Elara.API.Helpers;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Product;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Seller
{
    [ApiController]
    [Route("api/v1/sellers/me/products")]
    [Authorize(Roles = "Seller")]
    public class SellerProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public SellerProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQuery query)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);
            var result = await _productService.GetSellerProductsAsync(userId, query);

            return Ok(ApiResponse<PaginatedResponse<ProductListDto>>.SuccessResponse(result));
        }

        [HttpGet("{productId:long}")]
        public async Task<IActionResult> GetProduct(long productId)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);
            var result = await _productService.GetSellerProductByIdAsync(userId, productId);

            return Ok(ApiResponse<ProductDetailsDto>.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] SellerCreateProductDto dto)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);
            var result = await _productService.CreateSellerProductAsync(userId, dto);

            return CreatedAtAction(nameof(GetProduct), new { productId = result.Id }, ApiResponse<ProductDetailsDto>.SuccessResponse(result));
        }

        [HttpPut("{productId:long}")]
        public async Task<IActionResult> UpdateProduct(long productId, [FromBody] SellerUpdateProductDto dto)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);
            var result = await _productService.UpdateSellerProductAsync(userId, productId, dto);

            return Ok(ApiResponse<ProductDetailsDto>.SuccessResponse(result));
        }

        [HttpPatch("{productId:long}/status")]
        public async Task<IActionResult> UpdateStatus(long productId, [FromBody] UpdateProductStatusDto dto)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);
            await _productService.UpdateSellerProductStatusAsync(userId, productId, dto.IsActive);

            return Ok(ApiResponse<string>.SuccessResponse("Product status updated successfully."));
        }

        [HttpDelete("{productId:long}")]
        public async Task<IActionResult> DeleteProduct(long productId)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);
            await _productService.DeleteSellerProductAsync(userId, productId);

            return Ok(ApiResponse<string>.SuccessResponse("Product deleted successfully."));
        }

        [HttpPost("{productId:long}/images")]
        public async Task<IActionResult> AddImages(long productId, [FromForm] List<IFormFile> images)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);

            var requests = images.Select(image => new ImageUploadRequest
            {
                Stream = image.OpenReadStream(),
                FileName = image.FileName,
                ContentType = image.ContentType,
                Length = image.Length
            });

            var result = await _productService.AddSellerImagesAsync(userId, productId, requests);

            return Ok(ApiResponse<List<ProductImageDto>>.SuccessResponse(result));
        }

        [HttpPatch("{productId:long}/images/{imageId:long}/display-order")]
        public async Task<IActionResult> UpdateImageDisplayOrder(long productId, long imageId, [FromBody] UpdateImageDisplayOrderDto dto)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);

            await _productService.UpdateSellerImageDisplayOrderAsync(userId, productId, imageId, dto.DisplayOrder);

            return Ok(ApiResponse<string>.SuccessResponse("Image display order updated successfully."));
        }

        [HttpPatch("{productId:long}/images/reorder")]
        public async Task<IActionResult> ReorderImages(long productId, [FromBody] ReorderProductImagesDto dto)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);

            await _productService.ReorderSellerImagesAsync(userId, productId, dto.Images);

            return Ok(ApiResponse<string>.SuccessResponse("Images reordered successfully."));
        }

        [HttpDelete("{productId:long}/images/{imageId:long}")]
        public async Task<IActionResult> DeleteImage(long productId, long imageId)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);

            await _productService.DeleteSellerImageAsync(userId, productId, imageId);

            return Ok(ApiResponse<string>.SuccessResponse("Image deleted successfully."));
        }
    }
}