using Elara.Application.DTOs;

namespace Elara.Application.Interfaces
{
    public interface ILocalizationService
    {
        Task<List<LanguageDto>> GetLanguagesAsync(CancellationToken cancellationToken = default);
        Task<OperationResultDto> CreateLanguageAsync(CreateLanguageRequestDto request, CancellationToken cancellationToken = default);
        Task<OperationResultDto> UpdateLanguageAsync(int id, UpdateLanguageRequestDto request, CancellationToken cancellationToken = default);

        Task<List<ResourceStringDto>> GetResourceStringsAsync(string languageCode, CancellationToken cancellationToken = default);
        Task<OperationResultDto> UpsertResourceStringAsync(UpsertResourceStringRequestDto request, CancellationToken cancellationToken = default);
        Task<OperationResultDto> DeleteResourceStringAsync(int id, CancellationToken cancellationToken = default);
    }
}