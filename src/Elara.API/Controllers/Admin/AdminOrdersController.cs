using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Order;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/v1/admin/orders")]
    [ApiController]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AdminOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] AdminOrderFilterDto filterDto)
        {
            var orders = await _orderService.GetAllOrdersAsync(filterDto);
            return Ok(ApiResponse<PaginatedResponse<AdminOrderListDto>>.SuccessResponse(orders));
        }

        [HttpGet("{orderId:long}")]
        public async Task<IActionResult> GetOrderById(long orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            return Ok(ApiResponse<AdminOrderDetailsDto>.SuccessResponse(order));
        }

        [HttpPatch("{orderId:long}/status")]
        public async Task<IActionResult> UpdateOrderStatus(long orderId, [FromBody]UpdateOrderStatusRequestDto requestDto)
        {
            await _orderService.UpdateOrderStatusAsync(orderId, requestDto);
            return Ok(ApiResponse<string>.SuccessResponse("Order status updated successfully."));
        }
    }
}
