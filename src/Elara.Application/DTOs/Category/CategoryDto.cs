namespace Elara.Application.DTOs.Category
{
    public class CategoryDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public long? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
