using Api.Data;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories.Implementations
{
    public class EnergieRepository : IEnergieRepository
    {
        private readonly DataContext _context;

        public EnergieRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Energie>> GetAllAsync()
        {
            return await _context.Energies
                .Include(e => e.Site)
                .ToListAsync();
        }

        public async Task<Energie?> GetByIdAsync(int id)
        {
            return await _context.Energies
                .Include(e => e.Site)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Energie>> GetBySiteIdAsync(int siteId)
        {
            return await _context.Energies
                .Where(e => e.SiteId == siteId)
                .ToListAsync();
        }

        public async Task<Energie> CreateAsync(Energie energie)
        {
            _context.Energies.Add(energie);
            await _context.SaveChangesAsync();
            return energie;
        }

        public async Task<Energie?> UpdateAsync(Energie energie)
        {
            var existingEnergie = await _context.Energies.FindAsync(energie.Id);
            if (existingEnergie == null)
                return null;

            _context.Entry(existingEnergie).CurrentValues.SetValues(energie);
            await _context.SaveChangesAsync();
            return existingEnergie;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var energie = await _context.Energies.FindAsync(id);
            if (energie == null)
                return false;

            _context.Energies.Remove(energie);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
