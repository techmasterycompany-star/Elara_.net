using Elara.API.Helpers;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Shipment;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Customer
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/v1")]
    public class CustomerShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public CustomerShipmentsController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpGet("orders/{orderId:long}/shipments")]
        public async Task<IActionResult> GetOrderShipments(long orderId)
        {
            var userId = User.GetAuthenticatedUserId();

            var result = await _shipmentService.GetCustomerShipmentsByOrderIdAsync(orderId, userId);

            return Ok(ApiResponse<IEnumerable<CustomerShipmentListDto>>.SuccessResponse(result));
        }

        [HttpGet("orders/{orderId:long}/shipments/{shipmentId:long}")]
        public async Task<IActionResult> GetShipment(long orderId, long shipmentId)
        {
            var userId = User.GetAuthenticatedUserId();

            var result = await _shipmentService.GetCustomerShipmentByIdAsync(orderId, shipmentId, userId);

            return Ok(ApiResponse<CustomerShipmentDetailsDto>.SuccessResponse(result));
        }
    }
}
