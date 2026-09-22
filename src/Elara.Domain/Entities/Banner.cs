using Elara.Domain.Common;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class Banner : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Subtitle { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string ImagePublicId { get; set; } = null!;
        public string? LinkUrl { get; set; }
        public BannerPosition Position { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ICollection<HomepageSection> HomepageSections { get; set; } = [];
    }
}
