using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Product;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Customer
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQuery query)
        {
            var products = await _productService.GetAdminProductsAsync(query);
            return Ok(ApiResponse<PaginatedResponse<ProductListDto>>.SuccessResponse(products));
        }

        [HttpGet("{productId:long}")]
        public async Task<IActionResult> GetProduct(long productId)
        {
            var product = await _productService.GetAdminProductByIdAsync(productId);
            return Ok(ApiResponse<ProductDetailsDto>.SuccessResponse(product));
        }
    }
}
