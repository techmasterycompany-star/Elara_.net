using Elara.Application.DTOs.Category;
using Elara.Application.DTOs.Common;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Customer
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] CategoryListRequest request)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(request);
            return Ok(ApiResponse<PaginatedResponse<CategoryDto>>.SuccessResponse(categories));
        }

        [HttpGet("{categoryId:long}")]
        public async Task<IActionResult> GetCategory(long categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            return Ok(ApiResponse<CategoryDto?>.SuccessResponse(category));
        }
    }
}
