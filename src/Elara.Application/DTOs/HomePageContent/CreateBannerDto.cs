using Elara.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Elara.Application.DTOs.HomePageContent
{
    public class CreateBannerDto
    {
        public string Title { get; set; } = null!;
        public string Subtitle { get; set; } = null!;
        public IFormFile Image { get; set; } = null!;
        public string? LinkUrl { get; set; }
        public BannerPosition Position { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
