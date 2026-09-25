using AutoMapper;
using Elara.Application.DTOs.Checkout;
using Elara.Application.DTOs.Payment;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;
using Elara.Domain.Enums;

namespace Elara.Application.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICheckoutRepository _checkoutRepository;
        private readonly IPaymentService _paymentService;
        private readonly IShipmentService _shipmentService;
        private readonly IMapper _mapper;

        public CheckoutService(
            ICheckoutRepository checkoutRepository,
            IPaymentService paymentService,
            IShipmentService shipmentService,
            IMapper mapper)
        {
            _checkoutRepository = checkoutRepository;
            _paymentService = paymentService;
            _shipmentService = shipmentService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShippingMethodDto>> GetShippingMethodsAsync()
        {
            var shippingMethods = await _checkoutRepository.GetActiveShippingMethodsAsync();
            return _mapper.Map<IEnumerable<ShippingMethodDto>>(shippingMethods);
        }

        public async Task<CheckoutPreviewResponse> PreviewCheckoutAsync(long? userId, string? guestSessionId, CheckoutPreviewRequest request)
        {
            // Get checkout items
            var checkoutItems = await GetCheckoutItemsAsync(userId, guestSessionId, request.CartId);

            // Validate shipping method
            var shippingMethod = await _checkoutRepository.GetShippingMethodByIdAsync(request.ShippingMethodId);
            if (shippingMethod == null)
                throw new NotFoundException("Shipping method not found");

            if (!shippingMethod.IsActive)
                throw new BadRequestException("Shipping method is not available");

            // Calculate subtotal
            decimal subtotal = checkoutItems.Sum(item => item.Subtotal);

            // Calculate discount
            decimal discountAmount = 0m;
            string? appliedPromoCode = null;

            if (!string.IsNullOrWhiteSpace(request.PromoCode))
            {
                var promoCode = await _checkoutRepository.GetPromoCodeByCodeAsync(request.PromoCode);
                if (promoCode != null && IsPromoCodeValid(promoCode))
                {
                    discountAmount = CalculateDiscount(subtotal, promoCode);
                    appliedPromoCode = promoCode.Code;
                }
            }

            decimal shippingCost = shippingMethod.BaseCost;

            decimal totalAmount = subtotal - discountAmount + shippingCost;

            return new CheckoutPreviewResponse
            {
                SubTotal = subtotal,
                DiscountAmount = discountAmount,
                ShippingCost = shippingCost,
                TotalAmount = totalAmount,
                Items = checkoutItems,
                AppliedPromoCode = appliedPromoCode,
                ShippingMethod = _mapper.Map<ShippingMethodDto>(shippingMethod)
            };
        }

        public async Task<CheckoutResponse> ProcessCheckoutAsync(long? userId, string? guestSessionId, CheckoutRequest request)
        {
            var checkoutItems = await GetCheckoutItemsAsync(userId, guestSessionId, request.CartId);
            await ValidateCheckoutItemsAsync(checkoutItems);

            var shippingMethod = await _checkoutRepository.GetShippingMethodByIdAsync(request.ShippingMethodId);
            if (shippingMethod == null)
                throw new NotFoundException("Shipping method not found");

            if (!shippingMethod.IsActive)
                throw new BadRequestException("Shipping method is not available");

            decimal subtotal = checkoutItems.Sum(x => x.Subtotal);
            decimal discountAmount = 0m;
            PromoCode? appliedPromoCode = null;

            if (!string.IsNullOrWhiteSpace(request.PromoCode))
            {
                appliedPromoCode = await _checkoutRepository.GetPromoCodeByCodeAsync(request.PromoCode);
                if (appliedPromoCode != null && IsPromoCodeValid(appliedPromoCode))
                    discountAmount = CalculateDiscount(subtotal, appliedPromoCode);
            }

            decimal shippingCost = shippingMethod.BaseCost;
            decimal totalAmount = subtotal - discountAmount + shippingCost;

            var order = new Order
            {
                UserId = userId,
                GuestFullName = userId.HasValue ? null : request.ShippingAddress.FullName,
                GuestPhoneNumber = userId.HasValue ? null : request.ShippingAddress.Phone,
                ShippingFullName = request.ShippingAddress.FullName,
                ShippingPhone = request.ShippingAddress.Phone,
                ShippingStreet = request.ShippingAddress.Street,
                ShippingCity = request.ShippingAddress.City,
                ShippingState = request.ShippingAddress.State,
                ShippingPostalCode = request.ShippingAddress.PostalCode,
                ShippingCountry = request.ShippingAddress.Country,
                ShippingMethodId = request.ShippingMethodId,
                PromoCodeId = appliedPromoCode?.Id,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                SubTotal = subtotal,
                DiscountAmount = discountAmount,
                ShippingCost = shippingCost,
                TotalAmount = totalAmount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            foreach (var item in checkoutItems)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    Subtotal = item.Subtotal,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            var payment = new Payment
            {
                Method = request.PaymentMethod,
                Provider = GetPaymentProvider(request.PaymentMethod),
                Amount = totalAmount,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            order.Payment = payment;

            Order? createdOrder = null;
            await _checkoutRepository.CreateTransactionAsync(async () =>
            {
                var existingOrder = await _checkoutRepository.GetOrderAsync(order);
                if (existingOrder != null)
                {
                    throw new BadRequestException("An order is already in progress for this user.");
                }
                createdOrder = await _checkoutRepository.CreateOrderAsync(order);
                await _checkoutRepository.UpdateStockAsync(checkoutItems);
            });

            await _shipmentService.CreateShipmentsForOrderAsync(createdOrder!.Id);

            var paymentResponse = await _paymentService.ProcessPaymentAsync(new PaymentRequestDto
            {
                OrderId = createdOrder.Id,
                PaymentMethod = request.PaymentMethod,
                Amount = totalAmount,
                ReturnUrl = request.ReturnUrl,
                CancelUrl = request.CancelUrl,
                PayPalEmail = request.PayPalEmail,
                CardDetails = request.CardDetails
            });

            await _checkoutRepository.ClearCartAsync(request.CartId.Value);

            return new CheckoutResponse
            {
                OrderId = createdOrder.Id,
                OrderNumber = $"ORD-{createdOrder.Id:D8}",
                OrderDate = createdOrder.OrderDate,
                TotalAmount = createdOrder.TotalAmount,
                PaymentMethod = payment.Method,
                PaymentStatus = paymentResponse.Status,
                CheckoutUrl = paymentResponse.RedirectUrl,
                ClientSecret = paymentResponse.ClientSecret,
                TransactionId = paymentResponse.TransactionId,
                PaymentProvider = paymentResponse.Provider,
                Message = paymentResponse.Message ?? (request.PaymentMethod == PaymentMethodType.CashOnDelivery
                    ? "Order placed successfully"
                    : "Payment action required")
            };
        }

        private async Task<List<CheckoutItemPreviewDto>> GetCheckoutItemsAsync(long? userId, string? guestSessionId, long? cartId)
        {
            List<CheckoutItemPreviewDto> checkoutItems = [];

            if (cartId.HasValue)
            {
                // Get items from cart
                var cart = await _checkoutRepository.GetCartWithItemsAsync(cartId.Value, userId, guestSessionId);
                if (cart == null)
                    throw new NotFoundException("Cart not found");

                if (!cart.Items.Any())
                    throw new BadRequestException("Cart is empty");

                foreach (var cartItem in cart.Items)
                {
                    var product = await _checkoutRepository.GetProductWithSellerAsync(cartItem.ProductId);
                    if (product == null) continue;

                    checkoutItems.Add(new CheckoutItemPreviewDto
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = cartItem.Quantity,
                        UnitPrice = product.Price,
                        Discount = 0m,
                        Subtotal = cartItem.Quantity * product.Price,
                        SellerName = product.SellerProfile?.StoreName ?? "Unknown Seller"
                    });
                }
            }
            else
            {
                throw new BadRequestException("No items provided for checkout");
            }

            if (!checkoutItems.Any())
                throw new BadRequestException("No valid items found for checkout");

            return checkoutItems;
        }

        private async Task ValidateCheckoutItemsAsync(List<CheckoutItemPreviewDto> items)
        {
            var productIds = items.Select(i => i.ProductId).ToList();
            var products = await _checkoutRepository.GetProductsWithSellerAsync(productIds);

            foreach (var item in items)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);

                // Validate product exists
                if (product == null)
                    throw new NotFoundException($"Product {item.ProductId} not found");

                // Validate product availability
                if (product.IsDeleted)
                    throw new BadRequestException($"Product '{product.Name}' is no longer available");

                if (!product.IsActive)
                    throw new BadRequestException($"Product '{product.Name}' is currently inactive");

                // Validate stock availability
                if (product.StockQuantity < item.Quantity)
                    throw new BadRequestException($"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}");

                // Validate seller availability
                if (product.SellerProfile == null || product.SellerProfile.IsDeleted)
                    throw new BadRequestException($"Seller for product '{product.Name}' is not available");

                if (!product.SellerProfile.IsApproved)
                    throw new BadRequestException($"Seller for product '{product.Name}' is not approved");

                item.UnitPrice = product.Price;
                item.Subtotal = item.Quantity * product.Price;
            }
        }

        private bool IsPromoCodeValid(PromoCode promoCode)
        {
            var now = DateTime.UtcNow;
            return promoCode.IsActive
                && promoCode.ExpiryDate >= now
                && promoCode.UsageLimit > promoCode.TimesUsed;
        }

        private decimal CalculateDiscount(decimal subtotal, PromoCode promoCode)
        {
            if (promoCode.DiscountType == DiscountType.Percentage)
            {
                if ((promoCode.DiscountValue > 0 && promoCode.DiscountValue < 100))
                {
                    return subtotal * (promoCode.DiscountValue / 100m);
                }
                throw new BadRequestException("Invalid percentage discount value");
            }
            else
            {
                if (promoCode.DiscountValue >= subtotal)
                {
                    throw new BadRequestException("Discount value cannot exceed subtotal");
                }
                return Math.Min(promoCode.DiscountValue, subtotal);
            }
        }

        private string GetPaymentProvider(PaymentMethodType paymentMethod)
        {
            return paymentMethod switch
            {
                PaymentMethodType.CreditCard => "Stripe",
                PaymentMethodType.PayPal => "PayPal",
                PaymentMethodType.CashOnDelivery => "COD",
                PaymentMethodType.Wallet => "Wallet",
                _ => "Unknown"
            };
        }

    }
}
