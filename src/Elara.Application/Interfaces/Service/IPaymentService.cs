using Elara.Application.DTOs.Payment;

namespace Elara.Application.Interfaces.Service
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> InitiatePaymentAsync(long? userId, PaymentRequestDto request);
        Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto request);
        Task<PaymentResponseDto> GetPaymentByIdAsync(long paymentId);
        Task<PaymentResponseDto> GetPaymentByOrderIdAsync(long orderId);
        Task<PaymentResponseDto> HandleWebhookAsync(PaymentWebhookDto webhook);
        Task<bool> RefundPaymentAsync(long paymentId, decimal? amount = null, string? reason = null);
        Task<IEnumerable<PaymentResponseDto>> GetUserPaymentsAsync(long userId);
        Task<PaymentResponseDto> RetryFailedPaymentAsync(long paymentId);
    }
}