using AutoMapper;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.ShippingMethods;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;

namespace Elara.Application.Services
{
    public class ShippingMethodService : IShippingMethodService
    {
        private readonly IShippingMethodRepository _shippingMethodRepository;
        private readonly IMapper _mapper;

        public ShippingMethodService(IShippingMethodRepository shippingMethodRepository, IMapper mapper)
        {
            _shippingMethodRepository = shippingMethodRepository;
            _mapper = mapper;
        }
        public async Task<PaginatedResponse<ShippingMethodDto>> GetAllShippingMethodsAsync(ShippingMethodListRequest shippingMethodRequest)
        {
            var shippingMethods = await _shippingMethodRepository.GetAllShippingMethodsAsync(shippingMethodRequest);
            var shippingMethodsDtos = _mapper.Map<IEnumerable<ShippingMethodDto>>(shippingMethods.Items).ToList();

            return new PaginatedResponse<ShippingMethodDto>
            {
                Data = shippingMethodsDtos,
                PageNumber = shippingMethodRequest.PageNumber,
                Limit = shippingMethodRequest.Limit,
                TotalCount = shippingMethods.TotalCount,
                TotalPages = (int)Math.Ceiling((double)shippingMethods.TotalCount / shippingMethodRequest.Limit)
            };
        }

        public async Task<ShippingMethodDto?> GetShippingMethodByIdAsync(long id)
        {
            var shippingMethod = await _shippingMethodRepository.GetShippingMethodByIdAsync(id);
            if (shippingMethod == null)
                throw new NotFoundException($"Shipping method not found.");

            return _mapper.Map<ShippingMethodDto?>(shippingMethod);
        }
        public async Task CreateShippingMethodAsync(CreateShippingMethodDto shippingMethod)
        {
            var nameExists = await _shippingMethodRepository.ShippingMethodNameExistsAsync(shippingMethod.Name);
            if (nameExists)
                throw new ConflictException($"Shipping method with name '{shippingMethod.Name}' already exists.");

            var newShippingMethod = _mapper.Map<ShippingMethod>(shippingMethod);
            await _shippingMethodRepository.CreateShippingMethodAsync(newShippingMethod);
        }

        public async Task UpdateShippingMethodAsync(long id, UpdateShippingMethodDto shippingMethod)
        {
            var existingShippingMethod = await _shippingMethodRepository.GetShippingMethodByIdAsync(id);

            if (existingShippingMethod == null)
                throw new NotFoundException("Shipping method not found.");

            var nameExists = await _shippingMethodRepository.ShippingMethodNameExistsAsync(shippingMethod.Name, id);

            if (nameExists)
                throw new ConflictException($"Shipping method with name '{shippingMethod.Name}' already exists.");

            _mapper.Map(shippingMethod, existingShippingMethod);

            existingShippingMethod.UpdatedAt = DateTime.UtcNow;

            await _shippingMethodRepository.UpdateShippingMethodAsync(existingShippingMethod);
        }

        public async Task UpdateShippingMethodStatusAsync(long id, UpdateShippingMethodStatusDto shippingMethod)
        {
            var shippingMethodToUpdate = await _shippingMethodRepository.GetShippingMethodByIdAsync(id);
            if(shippingMethodToUpdate == null)
                throw new NotFoundException($"Shipping method not found.");
            
            if(shippingMethodToUpdate.IsActive == shippingMethod.IsActive)
                throw new ConflictException($"Shipping method status is already set to '{shippingMethod.IsActive}'.");

            shippingMethodToUpdate.IsActive = shippingMethod.IsActive;
            shippingMethodToUpdate.UpdatedAt = DateTime.UtcNow;
            await _shippingMethodRepository.UpdateShippingMethodAsync(shippingMethodToUpdate);
        }

        public async Task DeleteShippingMethodAsync(long id)
        {
            var shippingMethodToUpdate = await _shippingMethodRepository.GetShippingMethodByIdAsync(id);
            if (shippingMethodToUpdate == null)
                throw new NotFoundException($"Shipping method not found.");
            

            shippingMethodToUpdate.IsDeleted = true;
            shippingMethodToUpdate.UpdatedAt = DateTime.UtcNow;
            await _shippingMethodRepository.UpdateShippingMethodAsync(shippingMethodToUpdate);
        }
    }
}
