using Elara.Application.DTOs.User;

namespace Elara.Application.Interfaces.Service.Auth
{
    public interface IProfileService
    {
        Task<UserProfileDto> GetProfileAsync(long userId);
        Task<UserProfileDto> UpdateProfileAsync(long userId, UpdateProfileRequest request);
        Task ChangePasswordAsync(long userId, ChangePasswordRequest request);
        Task<IList<AddressDto>> GetAddressesAsync(long userId);
        Task<AddressDto> AddAddressAsync(long userId, AddAddressRequest request);
        Task RemoveAddressAsync(long userId, long addressId);
    }
}
