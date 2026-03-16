using Api.Data;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories.Implementations
{
    public class MateriauRepository : IMateriauRepository
    {
        private readonly DataContext _context;

        public MateriauRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Materiau>> GetAllAsync()
        {
            return await _context.Materiaux
                .Include(m => m.SiteMateriaux)
                    .ThenInclude(sm => sm.Site)
                .ToListAsync();
        }

        public async Task<Materiau?> GetByIdAsync(int id)
        {
            return await _context.Materiaux
                .Include(m => m.SiteMateriaux)
                    .ThenInclude(sm => sm.Site)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Materiau> CreateAsync(Materiau materiau)
        {
            _context.Materiaux.Add(materiau);
            await _context.SaveChangesAsync();
            return materiau;
        }

        public async Task<Materiau?> UpdateAsync(Materiau materiau)
        {
            var existingMateriau = await _context.Materiaux.FindAsync(materiau.Id);
            if (existingMateriau == null)
                return null;

            _context.Entry(existingMateriau).CurrentValues.SetValues(materiau);
            await _context.SaveChangesAsync();
            return existingMateriau;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var materiau = await _context.Materiaux.FindAsync(id);
            if (materiau == null)
                return false;

            _context.Materiaux.Remove(materiau);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Materiaux.AnyAsync(m => m.Id == id);
        }
    }
}
