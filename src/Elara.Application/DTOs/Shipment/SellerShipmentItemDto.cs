namespace Elara.Application.DTOs.Shipment
{
    public class SellerShipmentItemDto
    {
        public long OrderItemId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
