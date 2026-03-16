using Api.Data;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories.Implementations
{
    public class FacteurEnergieRepository : IFacteurEnergieRepository
    {
        private readonly DataContext _context;

        public FacteurEnergieRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FacteurEnergie>> GetAllAsync()
        {
            return await _context.FacteursEnergie.ToListAsync();
        }

        public async Task<FacteurEnergie?> GetByIdAsync(int id)
        {
            return await _context.FacteursEnergie.FindAsync(id);
        }

        public async Task<FacteurEnergie?> GetByTypeEnergieAsync(string typeEnergie)
        {
            return await _context.FacteursEnergie
                .FirstOrDefaultAsync(f => f.TypeEnergie == typeEnergie);
        }

        public async Task<FacteurEnergie> CreateAsync(FacteurEnergie facteurEnergie)
        {
            _context.FacteursEnergie.Add(facteurEnergie);
            await _context.SaveChangesAsync();
            return facteurEnergie;
        }

        public async Task<FacteurEnergie?> UpdateAsync(FacteurEnergie facteurEnergie)
        {
            var existingFacteur = await _context.FacteursEnergie.FindAsync(facteurEnergie.Id);
            if (existingFacteur == null)
                return null;

            _context.Entry(existingFacteur).CurrentValues.SetValues(facteurEnergie);
            await _context.SaveChangesAsync();
            return existingFacteur;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var facteurEnergie = await _context.FacteursEnergie.FindAsync(id);
            if (facteurEnergie == null)
                return false;

            _context.FacteursEnergie.Remove(facteurEnergie);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
