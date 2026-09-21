using Elara.Application.DTOs.User;

namespace Elara.Application.Interfaces.Service.Auth
{
    public interface IProfileService
    {
        Task<UserProfileDto> GetProfileAsync(long userId);
        Task DeleteProfileAsync(long userId);
        Task<UserProfileDto> UpdateProfileAsync(long userId, UpdateProfileRequest request);
        Task ChangePasswordAsync(long userId, ChangePasswordRequest request);
        Task<IList<AddressDto>> GetAddressesAsync(long userId);
        Task<AddressDto> AddAddressAsync(long userId, AddAddressRequest request);
        Task<AddressDto> UpdateAddressAsync(long userId,long addressId , UpdateAddressRequest request);
        Task RemoveAddressAsync(long userId, long addressId);
        Task SetDefaultAddressAsync(long userId, long addressId);
    }
}
