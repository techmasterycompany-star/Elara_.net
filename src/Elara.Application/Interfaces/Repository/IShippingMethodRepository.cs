using Elara.Application.DTOs.ShippingMethods;
using Elara.Application.DTOs.Common;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IShippingMethodRepository 
    { 
        Task<PaginationQueryResult<ShippingMethod>> GetAllShippingMethodsAsync(ShippingMethodListRequest shippingMethodRequest);
        Task<ShippingMethod?> GetShippingMethodByIdAsync(long id);
        Task<bool> ShippingMethodNameExistsAsync(string name, long? excludeId = null);
        Task CreateShippingMethodAsync(ShippingMethod shippingMethod);
        Task UpdateShippingMethodAsync(ShippingMethod shippingMethod);
    }
}
