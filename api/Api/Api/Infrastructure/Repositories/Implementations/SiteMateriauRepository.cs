using Api.Data;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories.Implementations
{
    public class SiteMateriauRepository : ISiteMateriauRepository
    {
        private readonly DataContext _context;

        public SiteMateriauRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SiteMateriau>> GetAllAsync()
        {
            return await _context.SiteMateriaux
                .Include(sm => sm.Site)
                .Include(sm => sm.Materiau)
                .ToListAsync();
        }

        public async Task<SiteMateriau?> GetByIdAsync(int id)
        {
            return await _context.SiteMateriaux
                .Include(sm => sm.Site)
                .Include(sm => sm.Materiau)
                .FirstOrDefaultAsync(sm => sm.Id == id);
        }

        public async Task<IEnumerable<SiteMateriau>> GetBySiteIdAsync(int siteId)
        {
            return await _context.SiteMateriaux
                .Include(sm => sm.Materiau)
                .Where(sm => sm.SiteId == siteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<SiteMateriau>> GetByMateriauIdAsync(int materiauId)
        {
            return await _context.SiteMateriaux
                .Include(sm => sm.Site)
                .Where(sm => sm.MateriauId == materiauId)
                .ToListAsync();
        }

        public async Task<SiteMateriau> CreateAsync(SiteMateriau siteMateriau)
        {
            _context.SiteMateriaux.Add(siteMateriau);
            await _context.SaveChangesAsync();
            return siteMateriau;
        }

        public async Task<SiteMateriau?> UpdateAsync(SiteMateriau siteMateriau)
        {
            var existingSiteMateriau = await _context.SiteMateriaux.FindAsync(siteMateriau.Id);
            if (existingSiteMateriau == null)
                return null;

            _context.Entry(existingSiteMateriau).CurrentValues.SetValues(siteMateriau);
            await _context.SaveChangesAsync();
            return existingSiteMateriau;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var siteMateriau = await _context.SiteMateriaux.FindAsync(id);
            if (siteMateriau == null)
                return false;

            _context.SiteMateriaux.Remove(siteMateriau);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
