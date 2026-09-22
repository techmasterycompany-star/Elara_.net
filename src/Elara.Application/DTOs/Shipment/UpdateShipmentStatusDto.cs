using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;


namespace Elara.Application.DTOs.Shipment
{
    public class UpdateShipmentStatusRequestDto
    {
        [Required]
        public ShipmentStatus Status { get; set; }

    }

    public class CustomerShipmentListDto
    {
        public long Id { get; set; }
        public long SellerProfileId { get; set; }
        public string Carrier { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public ShipmentStatus Status { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
    }


    public class CustomerShipmentDetailsDto
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long SellerProfileId { get; set; }

        public string Carrier { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public ShipmentStatus Status { get; set; }

        public DateTime? ShippedDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }

        public ICollection<CustomerShipmentItemDto> Items { get; set; } = [];
    }


    public class CustomerShipmentItemDto
    {
        public long OrderItemId { get; set; }
        public int Quantity { get; set; }
    }
}
