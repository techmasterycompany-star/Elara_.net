namespace Elara.Application.DTOs.Product
{
    public class ProductListDto
    {
        public long Id { get; set; }
        public long SellerProfileId { get; set; }
        public string StoreName { get; set; } = null!;

        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }

        public string? MainImageUrl { get; set; }
    }
}
