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
            var categoriesDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            return new PaginatedResponse<CategoryDto>
            {
                Data = categoriesDtos,
                PageNumber = categoryRequest.PageNumber,
                Limit = categoryRequest.Limit,
                TotalCount = categories.Count(),
                TotalPages = (int)Math.Ceiling((double)categories.Count() / categoryRequest.Limit)
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
            var nameExists = await _categoryRepository.CategoryNameExistsAsync(category.Name);
            if (nameExists)
                throw new ConflictException($"Category with name '{category.Name}' already exists.");
            await _categoryRepository.CreateCategoryAsync(_mapper.Map<Domain.Entities.Category>(category));
        }

        public async Task UpdateCategoryAsync(long categoryId, UpdateCategoryDto category)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (existingCategory == null)
                throw new NotFoundException($"Category not found.");

            var nameExists = await _categoryRepository.CategoryNameExistsAsync(category.Name);
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
