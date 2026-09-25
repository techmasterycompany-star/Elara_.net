using AutoMapper;
using Elara.Application.DTOs.Cart;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;

namespace Elara.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<GuestSessionDto> CreateGuestSessionAsync()
        {
            var guestSessionId = Guid.NewGuid().ToString();
            var cart = await _cartRepository.CreateGuestCartAsync(guestSessionId);

            return new GuestSessionDto
            {
                GuestSessionId = guestSessionId,
                CreatedAt = cart.CreatedAt
            };
        }

        public async Task<CartDto> GetCartItemsAsync(long? userId, string? guestSessionId)
        {
            await CheckAuthorizationAsync(userId, guestSessionId);
            var cart = await GetCartAsync(userId, guestSessionId);

            if (cart == null)
            {
                return new CartDto
                {
                    CartId = 0,
                    Items = [],
                    TotalItems = 0,
                    TotalAmount = 0
                };
            }

            var cartDto = _mapper.Map<CartDto>(cart);
            return cartDto;
        }

        public async Task<CartItemDto> AddToCartAsync(long? userId, string? guestSessionId, AddToCartDto addToCartDto)
        {
            await CheckAuthorizationAsync(userId, guestSessionId);
            if (addToCartDto.Quantity <= 0)
                throw new BadRequestException("Quantity must be greater than zero");

            var product = await _cartRepository.GetProductByIdAsync(addToCartDto.ProductId);
            if (product == null)
                throw new NotFoundException("Product not found");

            if (product.IsDeleted)
                throw new BadRequestException("Product is no longer available");

            if (!product.IsActive)
                throw new BadRequestException("Product is currently inactive");

            if (product.StockQuantity < addToCartDto.Quantity)
                throw new BadRequestException($"Insufficient stock. Available: {product.StockQuantity}");

            // Get or create cart
            var cart = await GetCartAsync(userId, guestSessionId);
            if (cart == null)
            {
                cart = userId.HasValue
                    ? await _cartRepository.CreateCartAsync(userId.Value)
                    : await _cartRepository.CreateGuestCartAsync(guestSessionId!);
            }

            // Check if product already exists in cart
            var existingCartItem = await _cartRepository.GetCartItemAsync(cart.Id, addToCartDto.ProductId);

            if (existingCartItem != null)
            {
                // Update existing cart item quantity
                var newQuantity = existingCartItem.Quantity + addToCartDto.Quantity;

                // Validate new quantity against stock
                if (product.StockQuantity < newQuantity)
                    throw new BadRequestException($"Insufficient stock. Available: {product.StockQuantity}, Current in cart: {existingCartItem.Quantity}");

                existingCartItem.Quantity = newQuantity;
                existingCartItem.UpdatedAt = DateTime.UtcNow;

                await _cartRepository.UpdateCartItemAsync(existingCartItem);

                return _mapper.Map<CartItemDto>(existingCartItem);
            }
            else
            {
                // Add new cart item
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = addToCartDto.ProductId,
                    Quantity = addToCartDto.Quantity,
                    UnitPriceSnapshot = product.Price,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var addedCartItem = await _cartRepository.AddCartItemAsync(cartItem);
                return _mapper.Map<CartItemDto>(addedCartItem);
            }
        }

        public async Task RemoveFromCartAsync(long? userId, string? guestSessionId, long productId)
        {
            await CheckAuthorizationAsync(userId, guestSessionId);
            var cart = await GetCartAsync(userId, guestSessionId);
            if (cart == null)
                throw new NotFoundException("Cart not found");

            // Get cart item
            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (cartItem == null)
                throw new NotFoundException("Product not found in cart");

            await _cartRepository.RemoveCartItemAsync(cartItem);
        }

        public async Task<CartItemDto> UpdateCartItemQuantityAsync(long? userId, string? guestSessionId, long productId, int quantity)
        {
            await CheckAuthorizationAsync(userId, guestSessionId);
            if (quantity <= 0)
                throw new BadRequestException("Quantity must be greater than zero");

            var cart = await GetCartAsync(userId, guestSessionId);
            if (cart == null)
                throw new NotFoundException("Cart not found");

            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (cartItem == null)
                throw new NotFoundException("Product not found in cart");

            var product = await _cartRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new NotFoundException("Product not found");

            if (product.IsDeleted)
                throw new BadRequestException("Product is no longer available");

            if (!product.IsActive)
                throw new BadRequestException("Product is currently inactive");

            if (product.StockQuantity < quantity)
                throw new BadRequestException($"Insufficient stock. Available: {product.StockQuantity}");

            // Update quantity
            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.UpdateCartItemAsync(cartItem);

            return _mapper.Map<CartItemDto>(cartItem);
        }

        private async Task<Cart?> GetCartAsync(long? userId, string? guestSessionId)
        {
            await CheckAuthorizationAsync(userId, guestSessionId);
            if (userId.HasValue)
                return await _cartRepository.GetCartByUserIdAsync(userId.Value);

            if (string.IsNullOrWhiteSpace(guestSessionId))
                throw new BadRequestException("Guest session ID is required");

            return await _cartRepository.GetCartByGuestSessionIdAsync(guestSessionId);
        }

        private async Task CheckAuthorizationAsync(long? userId, string? guestSessionId)
        {
            if (userId == null && string.IsNullOrWhiteSpace(guestSessionId))
                throw new UnauthorizedAccessException("Either user ID or guest session ID must be provided");
        }
    }
}
