namespace Elara.Application.DTOs.Product
{
    public class SellerUpdateProductDto
    {
        public long CategoryId { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
