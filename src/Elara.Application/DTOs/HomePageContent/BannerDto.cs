using Elara.Domain.Enums;

namespace Elara.Application.DTOs.HomePageContent
{
    public class BannerDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Subtitle { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string? LinkUrl { get; set; }
        public BannerPosition Position { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
