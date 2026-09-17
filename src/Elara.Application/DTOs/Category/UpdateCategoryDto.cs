namespace Elara.Application.DTOs.Category
{
    public class UpdateCategoryDto
    {
        public string Name { get; set; } = null!;
        public long? ParentCategoryId { get; set; }
        public string Description { get; set; } = null!;
    }
}
