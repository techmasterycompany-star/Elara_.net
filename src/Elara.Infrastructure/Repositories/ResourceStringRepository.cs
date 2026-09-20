using Elara.Application.Interfaces;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class ResourceStringRepository : IResourceStringRepository
    {
        private readonly AppDbContext _context;

        public ResourceStringRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ResourceString>> GetByLanguageCodeAsync(string languageCode, CancellationToken cancellationToken = default)
        {
            return await _context.ResourceStrings
                .Where(r => r.Language.Code == languageCode)
                .ToListAsync(cancellationToken);
        }

        public async Task<ResourceString?> GetByKeyAndLanguageAsync(string key, int languageId, CancellationToken cancellationToken = default)
        {
            return await _context.ResourceStrings
                .FirstOrDefaultAsync(r => r.Key == key && r.LanguageId == languageId, cancellationToken);
        }

        public async Task<ResourceString?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ResourceStrings.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task AddAsync(ResourceString resourceString, CancellationToken cancellationToken = default)
        {
            await _context.ResourceStrings.AddAsync(resourceString, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(ResourceString resourceString, CancellationToken cancellationToken = default)
        {
            _context.ResourceStrings.Update(resourceString);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(ResourceString resourceString, CancellationToken cancellationToken = default)
        {
            _context.ResourceStrings.Remove(resourceString);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}