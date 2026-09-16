using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;


namespace Elara.Application.DTOs.Shipment
{
    public class UpdateShipmentStatusRequestDto
    {
        [Required]
        public ShipmentStatus Status { get; set; }

    }
}
