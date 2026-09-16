namespace Elara.Application.DTOs.Inventory
{
    public class ProductStockDto
    {
        public long ProductId { get; set; }
        public string Name { get; set; } = null!;
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
    }
}
