using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Payment;
using Elara.Application.Interfaces.Service;
using Elara.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Payment
{
    [Route("api/v1/payments")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequestDto request)
        {
            var userId = User.GetAuthenticatedUserId();
            var payment = await _paymentService.InitiatePaymentAsync(userId, request);
            var response = ApiResponse<PaymentResponseDto>.SuccessResponse(payment);
            return Ok(response);
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto request)
        {
            var payment = await _paymentService.ProcessPaymentAsync(request);
            var response = ApiResponse<PaymentResponseDto>.SuccessResponse(payment);
            return Ok(response);
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(long paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            var response = ApiResponse<PaymentResponseDto>.SuccessResponse(payment);
            return Ok(response);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(long orderId)
        {
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            var response = ApiResponse<PaymentResponseDto>.SuccessResponse(payment);
            return Ok(response);
        }

        [HttpPost("retry/{paymentId}")]
        public async Task<IActionResult> RetryPayment(long paymentId)
        {
            var payment = await _paymentService.RetryFailedPaymentAsync(paymentId);
            var response = ApiResponse<PaymentResponseDto>.SuccessResponse(payment);
            return Ok(response);
        }

        [HttpPost("refund/{paymentId}")]
        public async Task<IActionResult> RefundPayment(long paymentId, [FromQuery] decimal? amount = null, [FromQuery] string? reason = null)
        {
            var result = await _paymentService.RefundPaymentAsync(paymentId, amount, reason);
            var response = ApiResponse<bool>.SuccessResponse(result);
            return Ok(response);
        }

        [HttpGet("my-payments")]
        public async Task<IActionResult> GetMyPayments()
        {
            var userId = User.GetAuthenticatedUserId();
            var payments = await _paymentService.GetUserPaymentsAsync(userId);
            var response = ApiResponse<IEnumerable<PaymentResponseDto>>.SuccessResponse(payments);
            return Ok(response);
        }
    }
}