namespace Elara.Application.DTOs.Product
{
    public class AdminCreateProductDto
    {
        public long SellerProfileId { get; set; }
        public long CategoryId { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
