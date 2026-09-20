using Elara.Domain.Entities;

namespace Elara.Application.Interfaces
{
    public interface ILanguageRepository
    {
        Task<List<Language>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Language?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Language?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task AddAsync(Language language, CancellationToken cancellationToken = default);
        Task UpdateAsync(Language language, CancellationToken cancellationToken = default);
    }
}