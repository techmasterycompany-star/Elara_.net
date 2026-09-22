using Elara.Application.DTOs.User;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service.Auth;
using Elara.Domain.Entities;

namespace Elara.Application.Services.Auth
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository repo;
        private readonly IPasswordHasher passwordHasher;
        public ProfileService(IUserRepository repo, IPasswordHasher passwordHasher)
        {
            this.repo = repo;
            this.passwordHasher = passwordHasher;
        }

        public async Task<AddressDto> AddAddressAsync(long userId, AddAddressRequest request)
        {
            var user = await repo.GetByIdWithAddressesAsync(userId)
               ?? throw new KeyNotFoundException("User not found.");

            if (request.IsDefault)
            {
                foreach (var addr in user.Addresses)
                    addr.IsDefault = false;
            }

            var address = new Address
            {
                UserId = userId,
                Label = request.Label,
                Street = request.Street,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                IsDefault = request.IsDefault
            };

            user.Addresses.Add(address);
            await repo.UpdateUserAsync(user);

            return new AddressDto
            {
                Id = address.Id,
                Label = address.Label,
                Street = address.Street,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                IsDefault = address.IsDefault
            };
        }

        public async Task ChangePasswordAsync(long userId, ChangePasswordRequest request)
        {
            var user = await repo.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
                throw new InvalidOperationException("Current password is incorrect.");

            user.PasswordHash = passwordHasher.Hash(request.NewPassword);
            await repo.UpdateUserAsync(user);
        }

        public async Task DeleteProfileAsync(long userId)
        {
            var user = await repo.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            user.IsDeleted = true;
            user.IsActive = false;

            await repo.UpdateUserAsync(user);
        }

        public async Task<IList<AddressDto>> GetAddressesAsync(long userId)
        {
            var user = await repo.GetByIdWithAddressesAsync(userId)
               ?? throw new KeyNotFoundException("User not found.");

            return user.Addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                Label = a.Label,
                Street = a.Street,
                City = a.City,
                State = a.State,
                PostalCode = a.PostalCode,
                Country = a.Country,
                IsDefault = a.IsDefault
            }).ToList();
        }

        public async Task<UserProfileDto> GetProfileAsync(long userId)
        {
            var user = await repo.GetByIdWithAddressesAsync(userId)
               ?? throw new KeyNotFoundException("User not found.");

            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                EmailConfirmed = user.EmailConfirmed,
                IsActive = user.IsActive,
                Addresses = user.Addresses.Select(a => new AddressDto
                {
                    Id = a.Id,
                    Label = a.Label,
                    Street = a.Street,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.PostalCode,
                    Country = a.Country,
                    IsDefault = a.IsDefault
                }).ToList()
            };
        }

        public async Task RemoveAddressAsync(long userId, long addressId)
        {
            var user = await repo.GetByIdWithAddressesAsync(userId)
               ?? throw new KeyNotFoundException("User not found.");

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId)
                ?? throw new KeyNotFoundException("Address not found.");

            await repo.DeleteAddressAsync(address);
        }

        public async Task SetDefaultAddressAsync(long userId, long addressId)
        {
            var user = await repo.GetByIdWithAddressesAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId)
                ?? throw new KeyNotFoundException("Address not found.");

            foreach (var item in user.Addresses)
            {
                item.IsDefault = item.Id == addressId;
            }

            await repo.UpdateUserAsync(user);
        }

        public async Task<AddressDto> UpdateAddressAsync(long userId, long addressId, UpdateAddressRequest request)
        {
            var user = await repo.GetByIdWithAddressesAsync(userId)
               ?? throw new KeyNotFoundException("User not found.");

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId)
                ?? throw new KeyNotFoundException("Address not found.");

            address.Label = request.Label;
            address.Street = request.Street;
            address.City = request.City;
            address.State = request.State;
            address.PostalCode = request.PostalCode;
            address.Country = request.Country;

            await repo.UpdateUserAsync(user);

            return new AddressDto
            {
                Id = address.Id,
                Label = address.Label,
                Street = address.Street,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                IsDefault = address.IsDefault
            };
        }

        public async Task<UserProfileDto> UpdateProfileAsync(long userId, UpdateProfileRequest request)
        {
            var user = await repo.GetByIdAsync(userId)
               ?? throw new KeyNotFoundException("User not found.");

            if (request.FullName != null)
                user.FullName = request.FullName;

            if (request.PhoneNumber != null)
                user.PhoneNumber = request.PhoneNumber;

            await repo.UpdateUserAsync(user);

            return await GetProfileAsync(userId);
        }
    }
}
