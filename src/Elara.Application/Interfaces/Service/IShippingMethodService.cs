using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.ShippingMethods;

namespace Elara.Application.Interfaces.Service
{
    public interface IShippingMethodService
    {
        Task<PaginatedResponse<ShippingMethodDto>> GetAllShippingMethodsAsync(ShippingMethodListRequest shippingMethodRequest);
        Task<ShippingMethodDto?> GetShippingMethodByIdAsync(long id);
        Task CreateShippingMethodAsync(CreateShippingMethodDto shippingMethod);
        Task UpdateShippingMethodAsync(long id, UpdateShippingMethodDto shippingMethod);
        Task UpdateShippingMethodStatusAsync(long id, UpdateShippingMethodStatusDto shippingMethod);
        Task DeleteShippingMethodAsync(long id);
    }
}
