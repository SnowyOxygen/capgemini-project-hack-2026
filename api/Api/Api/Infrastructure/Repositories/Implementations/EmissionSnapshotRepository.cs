using Api.Data;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories.Implementations
{
    public class EmissionSnapshotRepository : IEmissionSnapshotRepository
    {
        private readonly DataContext _context;

        public EmissionSnapshotRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmissionSnapshot>> GetAllAsync()
        {
            return await _context.EmissionSnapshots
                .Include(es => es.Site)
                .OrderByDescending(es => es.PeriodeDebut)
                .ToListAsync();
        }

        public async Task<EmissionSnapshot?> GetByIdAsync(int id)
        {
            return await _context.EmissionSnapshots
                .Include(es => es.Site)
                .FirstOrDefaultAsync(es => es.Id == id);
        }

        public async Task<IEnumerable<EmissionSnapshot>> GetBySiteIdAsync(int siteId)
        {
            return await _context.EmissionSnapshots
                .Where(es => es.SiteId == siteId)
                .OrderByDescending(es => es.PeriodeDebut)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmissionSnapshot>> GetBySiteIdAndDateRangeAsync(int siteId, DateTime dateDebut, DateTime dateFin)
        {
            return await _context.EmissionSnapshots
                .Where(es => es.SiteId == siteId
                    && es.PeriodeDebut >= dateDebut
                    && es.PeriodeFin <= dateFin)
                .OrderBy(es => es.PeriodeDebut)
                .ToListAsync();
        }

        public async Task<EmissionSnapshot?> GetLatestBySiteIdAsync(int siteId)
        {
            return await _context.EmissionSnapshots
                .Where(es => es.SiteId == siteId)
                .OrderByDescending(es => es.PeriodeFin)
                .FirstOrDefaultAsync();
        }

        public async Task<EmissionSnapshot> CreateAsync(EmissionSnapshot emissionSnapshot)
        {
            _context.EmissionSnapshots.Add(emissionSnapshot);
            await _context.SaveChangesAsync();
            return emissionSnapshot;
        }

        public async Task<EmissionSnapshot?> UpdateAsync(EmissionSnapshot emissionSnapshot)
        {
            var existing = await _context.EmissionSnapshots.FindAsync(emissionSnapshot.Id);
            if (existing == null)
                return null;

            _context.Entry(existing).CurrentValues.SetValues(emissionSnapshot);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var emissionSnapshot = await _context.EmissionSnapshots.FindAsync(id);
            if (emissionSnapshot == null)
                return false;

            _context.EmissionSnapshots.Remove(emissionSnapshot);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
