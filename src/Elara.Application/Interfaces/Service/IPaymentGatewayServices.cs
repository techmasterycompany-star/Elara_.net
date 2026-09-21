namespace Elara.Application.Interfaces.Service
{
    public interface IStripePaymentService
    {
        Task<StripePaymentResponse> CreatePaymentIntentAsync(StripePaymentRequest request);
        Task<bool> RefundAsync(string paymentIntentId, decimal? amount = null);
        Task<StripePaymentIntentDto?> GetPaymentIntentAsync(string paymentIntentId);
    }

    public interface IPayPalPaymentService
    {
        Task<PayPalPaymentResponse> CreateOrderAsync(PayPalPaymentRequest request);
        Task<bool> CaptureOrderAsync(string orderId);
        Task<bool> RefundAsync(string captureId, decimal? amount = null);
        Task<PayPalOrderDto?> GetOrderAsync(string orderId);
    }

    public interface IWalletService
    {
        Task<WalletBalanceDto> GetBalanceAsync(long userId);
        Task<WalletResult> DeductBalanceAsync(long userId, decimal amount);
        Task<WalletResult> AddBalanceAsync(long userId, decimal amount, string description);
        Task<WalletResult> RefundBalanceAsync(long userId, decimal amount);
        Task<IEnumerable<WalletTransactionDto>> GetTransactionsAsync(long userId);
    }

    // Internal DTOs
    public class StripePaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public long OrderId { get; set; }
        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
        public Elara.Application.DTOs.Payment.CardDetailsDto? CardDetails { get; set; }
    }

    public class StripePaymentResponse
    {
        public string PaymentIntentId { get; set; } = null!;
        public string? ClientSecret { get; set; }
        public string? RedirectUrl { get; set; }
        public bool RequiresAction { get; set; }
    }

    public class StripePaymentIntentDto
    {
        public string Id { get; set; } = null!;
        public string Status { get; set; } = null!;
        public long Amount { get; set; }
        public string Currency { get; set; } = null!;
        public Dictionary<string, string>? Metadata { get; set; }
        public string? ClientSecret { get; set; }
    }

    public class PayPalPaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public long OrderId { get; set; }
        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
        public string? PayerEmail { get; set; }
    }

    public class PayPalPaymentResponse
    {
        public string OrderId { get; set; } = null!;
        public string ApproveUrl { get; set; } = null!;
    }

    public class PayPalOrderDto
    {
        public string Id { get; set; } = null!;
        public string Status { get; set; } = null!;
        public PayPalPurchaseUnitDto[] PurchaseUnits { get; set; } = [];
    }

    public class PayPalPurchaseUnitDto
    {
        public PayPalAmountDto Amount { get; set; } = null!;
        public PayPalCaptureDto[] Payments { get; set; } = [];
    }

    public class PayPalAmountDto
    {
        public string Value { get; set; } = null!;
        public string CurrencyCode { get; set; } = null!;
    }

    public class PayPalCaptureDto
    {
        public string Id { get; set; } = null!;
        public string Status { get; set; } = null!;
        public PayPalAmountDto Amount { get; set; } = null!;
    }

    public class WalletBalanceDto
    {
        public long UserId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "USD";
    }

    public class WalletResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? TransactionId { get; set; }
    }

    public class WalletTransactionDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public WalletTransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public decimal BalanceAfter { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum WalletTransactionType
    {
        TopUp,
        Payment,
        Refund,
        Withdrawal,
        Bonus
    }
}