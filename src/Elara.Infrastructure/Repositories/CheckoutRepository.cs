using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class CheckoutRepository : ICheckoutRepository
    {
        private readonly AppDbContext _context;

        public CheckoutRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShippingMethod>> GetActiveShippingMethodsAsync()
        {
            return await _context.ShippingMethods
                .Where(sm => sm.IsActive && !sm.IsDeleted)
                .OrderBy(sm => sm.BaseCost)
                .ToListAsync();
        }

        public async Task<ShippingMethod?> GetShippingMethodByIdAsync(long shippingMethodId)
        {
            return await _context.ShippingMethods
                .FirstOrDefaultAsync(sm => sm.Id == shippingMethodId && !sm.IsDeleted);
        }

        public async Task<PromoCode?> GetPromoCodeByCodeAsync(string code)
        {
            return await _context.PromoCodes
                .FirstOrDefaultAsync(pc => pc.Code == code && !pc.IsDeleted);
        }

        public async Task<Cart?> GetCartWithItemsAsync(long cartId, long? userId, string? guestSessionId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.SellerProfile)
                .FirstOrDefaultAsync(c => c.Id == cartId &&
                    ((userId.HasValue && c.UserId == userId.Value) ||
                     (!userId.HasValue && c.GuestSessionId == guestSessionId)));
        }

        public async Task<Product?> GetProductWithSellerAsync(long productId)
        {
            return await _context.Products
                .Include(p => p.SellerProfile)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<IEnumerable<Product>> GetProductsWithSellerAsync(IEnumerable<long> productIds)
        {
            return await _context.Products
                .Include(p => p.SellerProfile)
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> ClearCartAsync(long cartId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task CreateTransactionAsync(Func<Task> action)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                await action();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
