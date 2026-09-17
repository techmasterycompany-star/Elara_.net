using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.ShippingMethods;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/v1/admin/shipping-methods")]
    [ApiController]
    public class AdminShippingMethodController : ControllerBase
    {
        private readonly IShippingMethodService _shippingMethodService;
        public AdminShippingMethodController(IShippingMethodService shippingMethodService)
        {
            _shippingMethodService = shippingMethodService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllShippingMethods([FromQuery] ShippingMethodListRequest listRequest)
        {
            var shippingMethods = await _shippingMethodService.GetAllShippingMethodsAsync(listRequest);
            return Ok(ApiResponse<PaginatedResponse<ShippingMethodDto>>.SuccessResponse(shippingMethods));
        }
        [HttpGet("{shippingMethodId:long}")]
        public async Task<IActionResult> GetShippingMethodById(long shippingMethodId)
        {
            var shippingMethod = await _shippingMethodService.GetShippingMethodByIdAsync(shippingMethodId);
            return Ok(ApiResponse<ShippingMethodDto>.SuccessResponse(shippingMethod));
        }
        [HttpPost]
        public async Task<IActionResult> CreateShippingMethod([FromBody] CreateShippingMethodDto createDto)
        {
            await _shippingMethodService.CreateShippingMethodAsync(createDto);
            return StatusCode(201, ApiResponse<string>.SuccessResponse("Shipping method created successfully."));
        }
        [HttpPut("{shippingMethodId:long}")]
        public async Task<IActionResult> UpdateShippingMethod(long shippingMethodId, [FromBody] UpdateShippingMethodDto updateDto)
        {
            await _shippingMethodService.UpdateShippingMethodAsync(shippingMethodId, updateDto);
            return StatusCode(204, ApiResponse<string>.SuccessResponse("Shipping method updated successfully."));
        }
        [HttpDelete("{shippingMethodId:long}")]
        public async Task<IActionResult> DeleteShippingMethod(long shippingMethodId)
        {
            await _shippingMethodService.DeleteShippingMethodAsync(shippingMethodId);
            return StatusCode(204, ApiResponse<string>.SuccessResponse("Shipping method deleted successfully."));
        }
    }
}
