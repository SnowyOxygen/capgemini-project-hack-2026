using Api.Data;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories.Implementations
{
    public class EmissionMensuelleRepository : IEmissionMensuelleRepository
    {
        private readonly DataContext _context;

        public EmissionMensuelleRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmissionMensuelle>> GetAllAsync()
        {
            return await _context.EmissionsMensuelles
                .Include(em => em.Site)
                .OrderByDescending(em => em.Annee)
                .ThenByDescending(em => em.Mois)
                .ToListAsync();
        }

        public async Task<EmissionMensuelle?> GetByIdAsync(int id)
        {
            return await _context.EmissionsMensuelles
                .Include(em => em.Site)
                .FirstOrDefaultAsync(em => em.Id == id);
        }

        public async Task<IEnumerable<EmissionMensuelle>> GetBySiteIdAsync(int siteId)
        {
            return await _context.EmissionsMensuelles
                .Where(em => em.SiteId == siteId)
                .OrderByDescending(em => em.Annee)
                .ThenByDescending(em => em.Mois)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmissionMensuelle>> GetBySiteIdAndYearAsync(int siteId, int annee)
        {
            return await _context.EmissionsMensuelles
                .Where(em => em.SiteId == siteId && em.Annee == annee)
                .OrderBy(em => em.Mois)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmissionMensuelle>> GetBySiteIdAndDateRangeAsync(int siteId, DateTime dateDebut, DateTime dateFin)
        {
            return await _context.EmissionsMensuelles
                .Where(em => em.SiteId == siteId 
                    && ((em.Annee > dateDebut.Year) || (em.Annee == dateDebut.Year && em.Mois >= dateDebut.Month))
                    && ((em.Annee < dateFin.Year) || (em.Annee == dateFin.Year && em.Mois <= dateFin.Month)))
                .OrderBy(em => em.Annee)
                .ThenBy(em => em.Mois)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmissionMensuelle>> GetLast12MonthsBySiteIdAsync(int siteId)
        {
            var now = DateTime.UtcNow;
            var twelveMonthsAgo = now.AddMonths(-12);

            return await _context.EmissionsMensuelles
                .Where(em => em.SiteId == siteId
                    && ((em.Annee > twelveMonthsAgo.Year) || (em.Annee == twelveMonthsAgo.Year && em.Mois >= twelveMonthsAgo.Month))
                    && ((em.Annee < now.Year) || (em.Annee == now.Year && em.Mois <= now.Month)))
                .OrderBy(em => em.Annee)
                .ThenBy(em => em.Mois)
                .ToListAsync();
        }

        public async Task<EmissionMensuelle?> GetBySiteIdYearMonthAsync(int siteId, int annee, int mois)
        {
            return await _context.EmissionsMensuelles
                .FirstOrDefaultAsync(em => em.SiteId == siteId && em.Annee == annee && em.Mois == mois);
        }

        public async Task<EmissionMensuelle> CreateAsync(EmissionMensuelle emissionMensuelle)
        {
            _context.EmissionsMensuelles.Add(emissionMensuelle);
            await _context.SaveChangesAsync();
            return emissionMensuelle;
        }

        public async Task<EmissionMensuelle?> UpdateAsync(EmissionMensuelle emissionMensuelle)
        {
            var existing = await _context.EmissionsMensuelles.FindAsync(emissionMensuelle.Id);
            if (existing == null)
                return null;

            _context.Entry(existing).CurrentValues.SetValues(emissionMensuelle);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var emissionMensuelle = await _context.EmissionsMensuelles.FindAsync(id);
            if (emissionMensuelle == null)
                return false;

            _context.EmissionsMensuelles.Remove(emissionMensuelle);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
