using Elara.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Elara.Application.DTOs.HomePageContent
{
    public class UpdateBannerDto
    {
        public string Title { get; set; } = null!;
        public string Subtitle { get; set; } = null!;
        public IFormFile? Image { get; set; }
        public string? LinkUrl { get; set; }
        public BannerPosition Position { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
