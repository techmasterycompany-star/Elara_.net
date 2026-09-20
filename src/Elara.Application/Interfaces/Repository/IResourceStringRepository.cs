using Elara.Domain.Entities;

namespace Elara.Application.Interfaces
{
    public interface IResourceStringRepository
    {
        Task<List<ResourceString>> GetByLanguageCodeAsync(string languageCode, CancellationToken cancellationToken = default);
        Task<ResourceString?> GetByKeyAndLanguageAsync(string key, int languageId, CancellationToken cancellationToken = default);
        Task<ResourceString?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(ResourceString resourceString, CancellationToken cancellationToken = default);
        Task UpdateAsync(ResourceString resourceString, CancellationToken cancellationToken = default);
        Task DeleteAsync(ResourceString resourceString, CancellationToken cancellationToken = default);
    }
}