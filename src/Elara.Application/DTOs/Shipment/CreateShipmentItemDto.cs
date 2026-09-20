namespace Elara.Application.DTOs.Shipment
{
    public class CreateShipmentItemDto
    {
        public long OrderItemId { get; set; }
        public int Quantity { get; set; }
    }
}
