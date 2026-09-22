using Elara.API.Helpers;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Order;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Seller
{
    [ApiController]
    [Route("api/v1/sellers/me/orders")]
    [Authorize(Roles = "Seller")]
    public class SellerOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public SellerOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] SellerOrderQuery query)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);
            var result = await _orderService.GetSellerOrdersAsync(userId, query);

            return Ok(ApiResponse<PaginatedResponse<SellerOrderListDto>>.SuccessResponse(result));
        }

        [HttpGet("{orderId:long}")]
        public async Task<IActionResult> GetOrder(long orderId)
        {
            var userId = ClaimsHelper.GetAuthenticatedUserId(User);
            var result = await _orderService.GetSellerOrderByIdAsync(orderId, userId);

            return Ok(ApiResponse<SellerOrderDetailsDto>.SuccessResponse(result));
        }
    }
}
