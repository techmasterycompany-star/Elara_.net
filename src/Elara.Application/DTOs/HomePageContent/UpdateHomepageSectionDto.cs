using Elara.Domain.Enums;

namespace Elara.Application.DTOs.HomePageContent
{
    public class UpdateHomepageSectionDto
    {
        public long? BannerId { get; set; }
        public long? CategoryId { get; set; }

        public string Title { get; set; } = null!;
        public string? SubTitle { get; set; }
        public HomepageSectionType Type { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public int MaxItems { get; set; }
    }
}
