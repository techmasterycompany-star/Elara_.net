using Elara.API.Helpers;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Order;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Customer
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/v1")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("users/me/orders")]
        public async Task<IActionResult> GetMyOrders([FromQuery] PaginationRequest request)
        {
            var userId = User.GetAuthenticatedUserId();

            var result = await _orderService.GetCustomerOrdersAsync(userId, request);

            return Ok(ApiResponse<PaginatedResponse<CustomerOrderListDto>>.SuccessResponse(result));
        }

        [HttpGet("orders/{orderId:long}")]
        public async Task<IActionResult> GetMyOrder(long orderId)
        {
            var userId = User.GetAuthenticatedUserId();

            var result = await _orderService.GetCustomerOrderByIdAsync(orderId, userId);

            return Ok(ApiResponse<CustomerOrderDetailsDto>.SuccessResponse(result));
        }

        [HttpPost("orders/{orderId:long}/cancel")]
        public async Task<IActionResult> CancelOrder(long orderId)
        {
            var userId = User.GetAuthenticatedUserId();

            await _orderService.CancelCustomerOrderAsync(orderId, userId);

            return Ok(ApiResponse<string>.SuccessResponse("Order cancelled successfully."));
        }

        [HttpGet("orders/{orderId:long}/status-history")]
        public async Task<IActionResult> GetOrderStatusHistory(long orderId)
        {
            var userId = User.GetAuthenticatedUserId();

            var result = await _orderService.GetOrderStatusHistoryAsync(orderId, userId);

            return Ok(ApiResponse<IEnumerable<CustomerOrderStatusHistoryDto>>.SuccessResponse(result));
        }
    }
}
