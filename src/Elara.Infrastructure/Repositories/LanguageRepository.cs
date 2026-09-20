using Elara.Application.Interfaces;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly AppDbContext _context;

        public LanguageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Language>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Languages.ToListAsync(cancellationToken);
        }

        public async Task<Language?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Languages.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<Language?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Languages.FirstOrDefaultAsync(l => l.Code == code, cancellationToken);
        }

        public async Task AddAsync(Language language, CancellationToken cancellationToken = default)
        {
            await _context.Languages.AddAsync(language, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Language language, CancellationToken cancellationToken = default)
        {
            _context.Languages.Update(language);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}