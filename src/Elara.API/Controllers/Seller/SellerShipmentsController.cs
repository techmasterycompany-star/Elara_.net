using Elara.API.Helpers;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Shipment;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Seller
{
    [ApiController]
    [Route("api/v1/seller")]
    [Authorize(Roles = "Seller")]
    public class SellerShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public SellerShipmentsController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpGet("shipments")]
        public async Task<IActionResult> GetShipments([FromQuery] SellerShipmentQuery query)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);
            var result = await _shipmentService.GetSellerShipmentsAsync(userId, query);

            return Ok(ApiResponse<PaginatedResponse<SellerShipmentListDto>>.SuccessResponse(result));
        }

        [HttpGet("shipments/{shipmentId:long}")]
        public async Task<IActionResult> GetShipment(long shipmentId)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);
            var result = await _shipmentService.GetSellerShipmentByIdAsync(shipmentId, userId);

            return Ok(ApiResponse<SellerShipmentDetailsDto>.SuccessResponse(result));
        }

        [HttpPost("orders/{orderId:long}/shipments")]
        public async Task<IActionResult> CreateShipment(long orderId, [FromBody] CreateSellerShipmentDto dto)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);
            var result = await _shipmentService.CreateSellerShipmentAsync(orderId, userId, dto);

            return CreatedAtAction(nameof(GetShipment), new { shipmentId = result.Id }, ApiResponse<SellerShipmentDetailsDto>.SuccessResponse(result));
        }

        [HttpPatch("shipments/{shipmentId:long}")]
        public async Task<IActionResult> UpdateShipment(long shipmentId, [FromBody] UpdateSellerShipmentDto dto)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);

            await _shipmentService.UpdateSellerShipmentAsync(shipmentId, userId, dto);

            return Ok(ApiResponse<string>.SuccessResponse("Shipment updated successfully."));
        }
    }
}
