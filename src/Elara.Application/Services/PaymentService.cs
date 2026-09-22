using AutoMapper;
using Elara.Application.DTOs.Payment;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;
using Elara.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace Elara.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IStripePaymentService _stripeService;
        private readonly IPayPalPaymentService _payPalService;
        private readonly IWalletService _walletService;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            IWalletRepository walletRepository,
            IMapper mapper,
            IConfiguration configuration,
            IStripePaymentService stripeService,
            IPayPalPaymentService payPalService,
            IWalletService walletService)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _walletRepository = walletRepository;
            _mapper = mapper;
            _configuration = configuration;
            _stripeService = stripeService;
            _payPalService = payPalService;
            _walletService = walletService;
        }

        public async Task<PaymentResponseDto> InitiatePaymentAsync(long? userId, PaymentRequestDto request)
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
                throw new NotFoundException("Order not found");

            if (order.UserId != userId)
                throw new UnauthorizedAccessException("You don't have access to this order");

            var existingPayment = await _paymentRepository.GetByOrderIdAsync(request.OrderId);
            if (existingPayment != null && existingPayment.Status == PaymentStatus.Pending)
            {
                return await ProcessPaymentAsync(request);
            }

            request.Amount = order.TotalAmount;
            return await ProcessPaymentAsync(request);
        }

        public async Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto request)
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
                throw new NotFoundException("Order not found");

            if (order.Payment != null && order.Payment.Status == PaymentStatus.Completed)
                throw new ConflictException("Order is already paid");

            Payment payment;

            if (order.Payment != null && order.Payment.Status == PaymentStatus.Pending)
            {
                payment = order.Payment;
                payment.Method = request.PaymentMethod;
                payment.Provider = GetPaymentProvider(request.PaymentMethod);
                payment.Amount = request.Amount;
                payment.Currency = request.Currency ?? "USD";
                payment.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                payment = new Payment
                {
                    OrderId = request.OrderId,
                    Method = request.PaymentMethod,
                    Provider = GetPaymentProvider(request.PaymentMethod),
                    Amount = request.Amount,
                    Currency = request.Currency ?? "USD",
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                order.Payment = payment;
            }

            PaymentResponseDto result;

            switch (request.PaymentMethod)
            {
                case PaymentMethodType.CreditCard:
                    result = await ProcessStripePaymentAsync(payment, request);
                    break;
                case PaymentMethodType.PayPal:
                    result = await ProcessPayPalPaymentAsync(payment, request);
                    break;
                case PaymentMethodType.CashOnDelivery:
                    result = await ProcessCodPaymentAsync(payment, request);
                    break;
                case PaymentMethodType.Wallet:
                    result = await ProcessWalletPaymentAsync(payment, request);
                    break;
                default:
                    throw new BadRequestException("Unsupported payment method");
            }

            if (result.Status == PaymentStatus.Completed)
            {
                await CompleteOrderAsync(order, payment);
            }

            await _paymentRepository.UpdateAsync(payment);
            await _orderRepository.UpdateOrderAsync(order);

            return result;
        }

        private async Task<PaymentResponseDto> ProcessStripePaymentAsync(Payment payment, PaymentRequestDto request)
        {
            var stripeRequest = new Elara.Application.Interfaces.Service.StripePaymentRequest
            {
                Amount = payment.Amount,
                Currency = payment.Currency,
                OrderId = payment.OrderId,
                ReturnUrl = request.ReturnUrl,
                CancelUrl = request.CancelUrl,
                CardDetails = request.CardDetails
            };

            var stripeResponse = await _stripeService.CreatePaymentIntentAsync(stripeRequest);

            payment.TransactionId = stripeResponse.PaymentIntentId;
            payment.Status = stripeResponse.RequiresAction ? PaymentStatus.Pending : PaymentStatus.Completed;
            payment.PaidAt = stripeResponse.RequiresAction ? null : DateTime.UtcNow;

            return new PaymentResponseDto
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                OrderNumber = $"ORD-{payment.OrderId:D8}",
                PaymentMethod = payment.Method,
                Status = payment.Status,
                Amount = payment.Amount,
                Currency = payment.Currency,
                TransactionId = payment.TransactionId,
                Provider = payment.Provider,
                ClientSecret = stripeResponse.ClientSecret,
                RedirectUrl = stripeResponse.RedirectUrl,
                CreatedAt = payment.CreatedAt,
                PaidAt = payment.PaidAt
            };
        }

        private async Task<PaymentResponseDto> ProcessPayPalPaymentAsync(Payment payment, PaymentRequestDto request)
        {
            var paypalRequest = new Elara.Application.Interfaces.Service.PayPalPaymentRequest
            {
                Amount = payment.Amount,
                Currency = payment.Currency,
                OrderId = payment.OrderId,
                ReturnUrl = request.ReturnUrl,
                CancelUrl = request.CancelUrl,
                PayerEmail = request.PayPalEmail
            };

            var paypalResponse = await _payPalService.CreateOrderAsync(paypalRequest);

            payment.TransactionId = paypalResponse.OrderId;
            payment.Status = PaymentStatus.Pending;
            payment.UpdatedAt = DateTime.UtcNow;

            return new PaymentResponseDto
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                OrderNumber = $"ORD-{payment.OrderId:D8}",
                PaymentMethod = payment.Method,
                Status = payment.Status,
                Amount = payment.Amount,
                Currency = payment.Currency,
                TransactionId = payment.TransactionId,
                Provider = payment.Provider,
                RedirectUrl = paypalResponse.ApproveUrl,
                CreatedAt = payment.CreatedAt
            };
        }

        private async Task<PaymentResponseDto> ProcessCodPaymentAsync(Payment payment, PaymentRequestDto request)
        {
            payment.Status = PaymentStatus.Completed;
            payment.Provider = "COD";
            payment.PaidAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            return new PaymentResponseDto
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                OrderNumber = $"ORD-{payment.OrderId:D8}",
                PaymentMethod = payment.Method,
                Status = payment.Status,
                Amount = payment.Amount,
                Currency = payment.Currency,
                TransactionId = payment.TransactionId,
                Provider = payment.Provider,
                CreatedAt = payment.CreatedAt,
                PaidAt = payment.PaidAt,
                Message = "Cash on Delivery - Payment will be collected upon delivery"
            };
        }

        private async Task<PaymentResponseDto> ProcessWalletPaymentAsync(Payment payment, PaymentRequestDto request)
        {
            var userId = (await _orderRepository.GetOrderByIdAsync(payment.OrderId))?.UserId;
            if (!userId.HasValue)
                throw new BadRequestException("User not found for wallet payment");

            var walletResult = await _walletService.DeductBalanceAsync(userId.Value, payment.Amount);
            if (!walletResult.Success)
                throw new BadRequestException(walletResult.Message ?? "Insufficient wallet balance");

            payment.Status = PaymentStatus.Completed;
            payment.PaidAt = DateTime.UtcNow;
            payment.Provider = "Wallet";
            payment.TransactionId = walletResult.TransactionId;

            return new PaymentResponseDto
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                OrderNumber = $"ORD-{payment.OrderId:D8}",
                PaymentMethod = payment.Method,
                Status = payment.Status,
                Amount = payment.Amount,
                Currency = payment.Currency,
                TransactionId = payment.TransactionId,
                Provider = payment.Provider,
                CreatedAt = payment.CreatedAt,
                PaidAt = payment.PaidAt
            };
        }

        private async Task CompleteOrderAsync(Order order, Payment payment)
        {
            order.Status = OrderStatus.Confirmed;
            order.UpdatedAt = DateTime.UtcNow;

            order.StatusHistory.Add(new OrderStatusHistory
            {
                OrderId = order.Id,
                Status = OrderStatus.Confirmed.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public async Task<PaymentResponseDto> GetPaymentByIdAsync(long paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new NotFoundException("Payment not found");

            return MapToDto(payment);
        }

        public async Task<PaymentResponseDto> GetPaymentByOrderIdAsync(long orderId)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            if (payment == null)
                throw new NotFoundException("Payment not found for this order");

            return MapToDto(payment);
        }

        public async Task<PaymentResponseDto> HandleWebhookAsync(PaymentWebhookDto webhook)
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(webhook.TransactionId);
            if (payment == null)
                throw new NotFoundException("Payment not found for transaction");

            var order = await _orderRepository.GetOrderByIdAsync(payment.OrderId);
            if (order == null)
                throw new NotFoundException("Order not found");

            payment.Status = webhook.Status;
            payment.PaidAt = webhook.Status == PaymentStatus.Completed ? DateTime.UtcNow : payment.PaidAt;
            payment.UpdatedAt = DateTime.UtcNow;

            if (webhook.Status == PaymentStatus.Completed)
            {
                await CompleteOrderAsync(order, payment);
            }
            else if (webhook.Status == PaymentStatus.Failed || webhook.Status == PaymentStatus.Cancelled)
            {
                order.Status = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;
                order.StatusHistory.Add(new OrderStatusHistory
                {
                    OrderId = order.Id,
                    Status = OrderStatus.Cancelled.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await _paymentRepository.UpdateAsync(payment);
            await _orderRepository.UpdateOrderAsync(order);

            return MapToDto(payment);
        }

        public async Task<bool> RefundPaymentAsync(long paymentId, decimal? amount = null, string? reason = null)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new NotFoundException("Payment not found");

            if (payment.Status != PaymentStatus.Completed)
                throw new BadRequestException("Can only refund completed payments");

            bool refunded = false;
            switch (payment.Method)
            {
                case PaymentMethodType.CreditCard:
                    refunded = await _stripeService.RefundAsync(payment.TransactionId!, amount);
                    break;
                case PaymentMethodType.PayPal:
                    refunded = await _payPalService.RefundAsync(payment.TransactionId!, amount);
                    break;
                case PaymentMethodType.Wallet:
                    var userId = (await _orderRepository.GetOrderByIdAsync(payment.OrderId))?.UserId;
                    if (userId.HasValue)
                    {
                        var walletResult = await _walletService.RefundBalanceAsync(userId.Value, amount ?? payment.Amount);
                        refunded = walletResult.Success;
                    }
                    break;
                case PaymentMethodType.CashOnDelivery:
                    refunded = true;
                    break;
            }

            if (refunded)
            {
                payment.Status = amount.HasValue && amount < payment.Amount
                    ? PaymentStatus.PartiallyRefunded
                    : PaymentStatus.Refunded;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);
            }

            return refunded;
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetUserPaymentsAsync(long userId)
        {
            var payments = await _paymentRepository.GetByUserIdAsync(userId);
            return payments.Select(MapToDto);
        }

        public async Task<PaymentResponseDto> RetryFailedPaymentAsync(long paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new NotFoundException("Payment not found");

            if (payment.Status != PaymentStatus.Failed && payment.Status != PaymentStatus.Cancelled)
                throw new BadRequestException("Can only retry failed or cancelled payments");

            var request = new PaymentRequestDto
            {
                OrderId = payment.OrderId,
                PaymentMethod = payment.Method,
                Amount = payment.Amount,
                Currency = payment.Currency
            };

            return await ProcessPaymentAsync(request);
        }

        private string GetPaymentProvider(PaymentMethodType method)
        {
            return method switch
            {
                PaymentMethodType.CreditCard => "Stripe",
                PaymentMethodType.PayPal => "PayPal",
                PaymentMethodType.CashOnDelivery => "COD",
                PaymentMethodType.Wallet => "Wallet",
                _ => "Unknown"
            };
        }

        private PaymentResponseDto MapToDto(Payment payment)
        {
            return new PaymentResponseDto
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                OrderNumber = $"ORD-{payment.OrderId:D8}",
                PaymentMethod = payment.Method,
                Status = payment.Status,
                Amount = payment.Amount,
                Currency = payment.Currency,
                TransactionId = payment.TransactionId,
                Provider = payment.Provider,
                CreatedAt = payment.CreatedAt,
                PaidAt = payment.PaidAt,
                Order = payment.Order != null ? new OrderSummaryDto
                {
                    Id = payment.Order.Id,
                    OrderNumber = $"ORD-{payment.Order.Id:D8}",
                    TotalAmount = payment.Order.TotalAmount,
                    PaymentStatus = payment.Order.Payment?.Status ?? PaymentStatus.Pending,
                    OrderStatus = payment.Order.Status
                } : null
            };
        }
    }
}