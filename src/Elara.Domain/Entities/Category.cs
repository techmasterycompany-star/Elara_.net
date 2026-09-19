using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class Category : SoftDelete
    {
        public string Name { get; set; } = null!;
        public long? ParentCategoryId { get; set; }

        public Category? ParentCategory { get; set; }
        public ICollection<Category> Children { get; set; } = [];

        public string Description { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = [];
        public ICollection<HomepageSection> HomepageSections { get; set; } = [];
    }
}
