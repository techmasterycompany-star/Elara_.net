using Elara.Application.DTOs.Category;
using Elara.Application.DTOs.Common;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Admin
{
    [Route("api/v1/admin/categories")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminCategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public AdminCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync([FromQuery] CategoryListRequest categoryRequest)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(categoryRequest);
            return Ok(ApiResponse<PaginatedResponse<CategoryDto>>.SuccessResponse(categories));
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetCategoryByIdAsync(long id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(ApiResponse<CategoryDto?>.SuccessResponse(category));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryDto category)
        {
            await _categoryService.CreateCategoryAsync(category);
            return StatusCode(201, ApiResponse<string>.SuccessResponse("Category created successfully."));
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateCategoryAsync(long id, [FromBody] UpdateCategoryDto category)
        {
            await _categoryService.UpdateCategoryAsync(id, category);
            return Ok(ApiResponse<string>.SuccessResponse("Category updated successfully."));
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteCategoryAsync(long id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Category deleted successfully."));
        }
    }
}
