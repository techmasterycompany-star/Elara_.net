using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Product;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/products")]
    [Authorize(Roles = "Admin")]
    public class AdminProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public AdminProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQuery query)
        {
            var result = await _productService.GetAdminProductsAsync(query);

            return Ok(ApiResponse<PaginatedResponse<ProductListDto>>.SuccessResponse(result));
        }

        [HttpGet("{productId:long}")]
        public async Task<IActionResult> GetProduct(long productId)
        {
            var result = await _productService.GetAdminProductByIdAsync(productId);

            return Ok(ApiResponse<ProductDetailsDto>.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] AdminCreateProductDto dto)
        {
            var result = await _productService.CreateAdminProductAsync(dto);

            return CreatedAtAction(nameof(GetProduct), new { productId = result.Id }, ApiResponse<ProductDetailsDto>.SuccessResponse(result));
        }

        [HttpPut("{productId:long}")]
        public async Task<IActionResult> UpdateProduct(long productId, [FromBody] AdminUpdateProductDto dto)
        {
            var result = await _productService.UpdateAdminProductAsync(productId, dto);

            return Ok(ApiResponse<ProductDetailsDto>.SuccessResponse(result));
        }

        [HttpPatch("{productId:long}/status")]
        public async Task<IActionResult> UpdateStatus(long productId, [FromBody] UpdateProductStatusDto dto)
        {
            await _productService.UpdateAdminProductStatusAsync(productId, dto.IsActive);

            return Ok(ApiResponse<string>.SuccessResponse("Product status updated successfully."));
        }

        [HttpDelete("{productId:long}")]
        public async Task<IActionResult> DeleteProduct(long productId)
        {
            await _productService.DeleteAdminProductAsync(productId);

            return Ok(ApiResponse<string>.SuccessResponse("Product deleted successfully."));
        }

        [HttpPost("{productId:long}/images")]
        public async Task<IActionResult> AddImages(long productId, [FromForm] List<IFormFile> images)
        {
            var requests = images.Select(image => new ImageUploadRequest
            {
                Stream = image.OpenReadStream(),
                FileName = image.FileName,
                ContentType = image.ContentType,
                Length = image.Length
            });

            var result = await _productService.AddAdminImagesAsync(productId, requests);

            return Ok(ApiResponse<List<ProductImageDto>>.SuccessResponse(result));
        }

        [HttpPatch("{productId:long}/images/{imageId:long}/display-order")]
        public async Task<IActionResult> UpdateImageDisplayOrder(long productId, long imageId, [FromBody] UpdateImageDisplayOrderDto dto)
        {
            await _productService.UpdateAdminImageDisplayOrderAsync(productId, imageId, dto.DisplayOrder);

            return Ok(ApiResponse<string>.SuccessResponse("Image display order updated successfully."));
        }

        [HttpPatch("{productId:long}/images/reorder")]
        public async Task<IActionResult> ReorderImages(long productId, [FromBody] ReorderProductImagesDto dto)
        {
            await _productService.ReorderAdminImagesAsync(productId, dto.Images);

            return Ok(ApiResponse<string>.SuccessResponse("Images reordered successfully."));
        }

        [HttpDelete("{productId:long}/images/{imageId:long}")]
        public async Task<IActionResult> DeleteImage(long productId, long imageId)
        {
            await _productService.DeleteAdminImageAsync(productId, imageId);

            return Ok(ApiResponse<string>.SuccessResponse("Image deleted successfully."));
        }
    }   

}
