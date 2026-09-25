using Elara.Application.DTOs;
using Elara.Application.Interfaces;
using Elara.Domain.Entities;

namespace Elara.Application.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IResourceStringRepository _resourceStringRepository;

        public LocalizationService(
            ILanguageRepository languageRepository,
            IResourceStringRepository resourceStringRepository)
        {
            _languageRepository = languageRepository;
            _resourceStringRepository = resourceStringRepository;
        }

        public async Task<List<LanguageDto>> GetLanguagesAsync(CancellationToken cancellationToken = default)
        {
            var languages = await _languageRepository.GetAllAsync(cancellationToken);

            return languages.Select(l => new LanguageDto
            {
                Id = l.Id,
                Code = l.Code,
                Name = l.Name,
                IsActive = l.IsActive,
                IsDefault = l.IsDefault
            }).ToList();
        }

        public async Task<OperationResultDto> CreateLanguageAsync(CreateLanguageRequestDto request, CancellationToken cancellationToken = default)
        {
            var existing = await _languageRepository.GetByCodeAsync(request.Code, cancellationToken);
            if (existing != null)
                return Invalid("A language with this code already exists.");

            var language = new Language
            {
                Code = request.Code,
                Name = request.Name,
                IsActive = true,
                IsDefault = request.IsDefault
            };

            await _languageRepository.AddAsync(language, cancellationToken);
            return new OperationResultDto { IsSuccess = true };
        }

        public async Task<OperationResultDto> UpdateLanguageAsync(int id, UpdateLanguageRequestDto request, CancellationToken cancellationToken = default)
        {
            var language = await _languageRepository.GetByIdAsync(id, cancellationToken);
            if (language == null)
                return Invalid("This language does not exist.");

            language.IsActive = request.IsActive;
            language.IsDefault = request.IsDefault;

            await _languageRepository.UpdateAsync(language, cancellationToken);
            return new OperationResultDto { IsSuccess = true };
        }

        public async Task<List<ResourceStringDto>> GetResourceStringsAsync(string languageCode, CancellationToken cancellationToken = default)
        {
            var resourceStrings = await _resourceStringRepository.GetByLanguageCodeAsync(languageCode, cancellationToken);

            return resourceStrings.Select(r => new ResourceStringDto
            {
                Id = r.Id,
                Key = r.Key,
                Value = r.Value
            }).ToList();
        }

        public async Task<OperationResultDto> UpsertResourceStringAsync(UpsertResourceStringRequestDto request, CancellationToken cancellationToken = default)
        {
            var existing = await _resourceStringRepository.GetByKeyAndLanguageAsync(
                request.Key, request.LanguageId, cancellationToken);

            if (existing != null)
            {
                existing.Value = request.Value;
                await _resourceStringRepository.UpdateAsync(existing, cancellationToken);
                return new OperationResultDto { IsSuccess = true };
            }

            var resourceString = new ResourceString
            {
                Key = request.Key,
                LanguageId = request.LanguageId,
                Value = request.Value
            };

            await _resourceStringRepository.AddAsync(resourceString, cancellationToken);
            return new OperationResultDto { IsSuccess = true };
        }

        public async Task<OperationResultDto> DeleteResourceStringAsync(int id, CancellationToken cancellationToken = default)
        {
            var resourceString = await _resourceStringRepository.GetByIdAsync(id, cancellationToken);
            if (resourceString == null)
                return Invalid("This resource string does not exist.");

            await _resourceStringRepository.DeleteAsync(resourceString, cancellationToken);
            return new OperationResultDto { IsSuccess = true };
        }

        private static OperationResultDto Invalid(string message) => new()
        {
            IsSuccess = false,
            ErrorMessage = message
        };
    }
}