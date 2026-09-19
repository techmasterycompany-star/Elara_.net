using AutoMapper;
using Elara.Application.DTOs.Category;
using Elara.Application.DTOs.Common;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;

namespace Elara.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<CategoryDto>> GetAllCategoriesAsync(CategoryListRequest categoryRequest)
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync(categoryRequest);
            var categoriesDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories.Items).ToList();

            return new PaginatedResponse<CategoryDto>
            {
                Data = categoriesDtos,
                PageNumber = categoryRequest.PageNumber,
                Limit = categoryRequest.Limit,
                TotalCount = categories.TotalCount,
                TotalPages = (int)Math.Ceiling((double)categories.TotalCount / categoryRequest.Limit)
            };
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(long id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
                throw new NotFoundException($"Category not found.");

            return _mapper.Map<CategoryDto?>(category);
        }
        public async Task CreateCategoryAsync(CreateCategoryDto category)
        {
            if (category.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetCategoryByIdAsync(category.ParentCategoryId.Value);
                if (parentCategory == null)
                    throw new NotFoundException("Parent category not found.");
            }

            var nameExists = await _categoryRepository.CategoryNameExistsAsync(category.Name);
            if (nameExists)
                throw new ConflictException($"Category with name '{category.Name}' already exists.");
            await _categoryRepository.CreateCategoryAsync(_mapper.Map<Category>(category));
        }

        public async Task UpdateCategoryAsync(long categoryId, UpdateCategoryDto category)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (existingCategory == null)
                throw new NotFoundException($"Category not found.");

            if (category.ParentCategoryId.HasValue)
            {
                if (category.ParentCategoryId.Value == categoryId)
                    throw new ConflictException("Category cannot be its own parent category.");

                var parentCategory = await _categoryRepository.GetCategoryByIdAsync(category.ParentCategoryId.Value);
                if (parentCategory == null)
                    throw new NotFoundException("Parent category not found.");
            }

            var nameExists = await _categoryRepository.CategoryNameExistsAsync(category.Name, categoryId);
            if (nameExists)
                throw new ConflictException($"Category with name '{category.Name}' already exists.");

            _mapper.Map(category, existingCategory);
            existingCategory.UpdatedAt = DateTime.UtcNow;
            await _categoryRepository.UpdateCategoryAsync(existingCategory);
        }
        public async Task DeleteCategoryAsync(long id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);  
            if(category == null)
                throw new NotFoundException($"Category not found.");

            category.IsDeleted = true;
            category.UpdatedAt = DateTime.UtcNow;
            await _categoryRepository.UpdateCategoryAsync(category);
        }    
        
    }
}
