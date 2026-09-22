using Elara.Domain.Common;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class HomepageSection : BaseEntity
    {
        public long? BannerId { get; set; }
        public Banner? Banner { get; set; }

        public string Title { get; set; } = null!;
        public string? SubTitle { get; set; }
        public HomepageSectionType Type { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public long? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int MaxItems { get; set; }
    }
}
