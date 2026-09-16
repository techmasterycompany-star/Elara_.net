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

        public async Task<CartDto> GetCartItemsAsync(long userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                // Return empty cart if none exists
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

        public async Task<CartItemDto> AddToCartAsync(long userId, AddToCartDto addToCartDto)
        {   
            // Validate quantity
            if (addToCartDto.Quantity <= 0)
                throw new BadRequestException("Quantity must be greater than zero");

            // Validate product exists and is available
            var product = await _cartRepository.GetProductByIdAsync(addToCartDto.ProductId);
            if (product == null)
                throw new NotFoundException("Product not found");

            if (product.IsDeleted)
                throw new BadRequestException("Product is no longer available");

            if (!product.IsActive)
                throw new BadRequestException("Product is currently inactive");

            // Validate stock availability
            if (product.StockQuantity < addToCartDto.Quantity)
                throw new BadRequestException($"Insufficient stock. Available: {product.StockQuantity}");

            // Get or create cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = await _cartRepository.CreateCartAsync(userId);
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

        public async Task RemoveFromCartAsync(long userId, long productId)
        {
            // Get user's cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new NotFoundException("Cart not found");

            // Get cart item
            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (cartItem == null)
                throw new NotFoundException("Product not found in cart");

            // Remove cart item
            await _cartRepository.RemoveCartItemAsync(cartItem.Id);
        }

        public async Task<CartItemDto> UpdateCartItemQuantityAsync(long userId, long productId, int quantity)
        {
            // Validate quantity
            if (quantity <= 0)
                throw new BadRequestException("Quantity must be greater than zero");

            // Get user's cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new NotFoundException("Cart not found");

            // Get cart item
            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (cartItem == null)
                throw new NotFoundException("Product not found in cart");

            // Validate product still exists and is available
            var product = await _cartRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new NotFoundException("Product not found");

            if (product.IsDeleted)
                throw new BadRequestException("Product is no longer available");

            if (!product.IsActive)
                throw new BadRequestException("Product is currently inactive");

            // Validate stock availability
            if (product.StockQuantity < quantity)
                throw new BadRequestException($"Insufficient stock. Available: {product.StockQuantity}");

            // Update quantity
            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.UpdateCartItemAsync(cartItem);

            return _mapper.Map<CartItemDto>(cartItem);
        }
    }
}
