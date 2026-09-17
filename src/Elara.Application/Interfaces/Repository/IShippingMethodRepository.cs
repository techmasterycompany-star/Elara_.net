using Elara.Application.DTOs.ShippingMethods;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IShippingMethodRepository 
    { 
        Task<IEnumerable<ShippingMethod>> GetAllShippingMethodsAsync(ShippingMethodListRequest shippingMethodRequest);
        Task<ShippingMethod?> GetShippingMethodByIdAsync(long id);
        Task<bool> ShippingMethodNameExistsAsync(string name, long? excludeId = null);
        Task CreateShippingMethodAsync(ShippingMethod shippingMethod);
        Task UpdateShippingMethodAsync(ShippingMethod shippingMethod);
    }
}
