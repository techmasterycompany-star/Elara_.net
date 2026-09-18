namespace Elara.Application.DTOs.SellerProfile
{
    public class SellerListDto
    {
        public long Id { get; set; }
        public string StoreName { get; set; } = null!;
        public string StoreDescription { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
