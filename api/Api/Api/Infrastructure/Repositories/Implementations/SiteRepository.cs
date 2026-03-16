using Api.Data;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories.Implementations
{
    public class SiteRepository : ISiteRepository
    {
        private readonly DataContext _context;

        public SiteRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Site>> GetAllAsync()
        {
            return await _context.Sites
                .Include(s => s.Parkings)
                .Include(s => s.Energies)
                .Include(s => s.SiteMateriaux)
                    .ThenInclude(sm => sm.Materiau)
                .ToListAsync();
        }

        public async Task<Site?> GetByIdAsync(int id)
        {
            return await _context.Sites
                .Include(s => s.Parkings)
                .Include(s => s.Energies)
                .Include(s => s.SiteMateriaux)
                    .ThenInclude(sm => sm.Materiau)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Site> CreateAsync(Site site)
        {
            _context.Sites.Add(site);
            await _context.SaveChangesAsync();
            return site;
        }

        public async Task<Site?> UpdateAsync(Site site)
        {
            var existingSite = await _context.Sites.FindAsync(site.Id);
            if (existingSite == null)
                return null;

            _context.Entry(existingSite).CurrentValues.SetValues(site);
            await _context.SaveChangesAsync();
            return existingSite;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var site = await _context.Sites.FindAsync(id);
            if (site == null)
                return false;

            _context.Sites.Remove(site);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Sites.AnyAsync(s => s.Id == id);
        }
    }
}
