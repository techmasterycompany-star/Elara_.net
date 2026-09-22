using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Shipment;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/v1/admin/shipments")]
    [ApiController]
    public class AdminShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public AdminShipmentsController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShipments([FromQuery] AdminShipmentFilterDto filterDto)
        {
            var shipments = await _shipmentService.GetAllShipmentsAsync(filterDto);
            return Ok(ApiResponse<PaginatedResponse<AdminShipmentListDto>>.SuccessResponse(shipments));
        }

        [HttpGet("{shipmentId:long}")]
        public async Task<IActionResult> GetShipmentById(long shipmentId)
        {
            var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
            return Ok(ApiResponse<AdminShipmentDetailsDto>.SuccessResponse(shipment));
        }
        [HttpPut("{shipmentId:long}")]
        public async Task<IActionResult> UpdateShipment(long shipmentId, [FromBody] AdminUpdateShipmentRequestDto requestDto)
        {
            await _shipmentService.UpdateShipmentAsync(shipmentId, requestDto);
            return Ok(ApiResponse<string>.SuccessResponse("Shipment updated successfully."));
        }

        [HttpPatch("{shipmentId:long}/status")]
        public async Task<IActionResult> UpdateShipmentStatus(long shipmentId, [FromBody]UpdateShipmentStatusRequestDto requestDto)
        {
            await _shipmentService.UpdateShipmentStatusAsync(shipmentId, requestDto);
            return Ok(ApiResponse<string>.SuccessResponse("Shipment status updated successfully."));
        }
    }
}
