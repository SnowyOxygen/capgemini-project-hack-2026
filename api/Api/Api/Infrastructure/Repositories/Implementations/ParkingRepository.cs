using Api.Data;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories.Implementations
{
    public class ParkingRepository : IParkingRepository
    {
        private readonly DataContext _context;

        public ParkingRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Parking>> GetAllAsync()
        {
            return await _context.Parkings
                .Include(p => p.Site)
                .ToListAsync();
        }

        public async Task<Parking?> GetByIdAsync(int id)
        {
            return await _context.Parkings
                .Include(p => p.Site)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Parking>> GetBySiteIdAsync(int siteId)
        {
            return await _context.Parkings
                .Where(p => p.SiteId == siteId)
                .ToListAsync();
        }

        public async Task<Parking> CreateAsync(Parking parking)
        {
            _context.Parkings.Add(parking);
            await _context.SaveChangesAsync();
            return parking;
        }

        public async Task<Parking?> UpdateAsync(Parking parking)
        {
            var existingParking = await _context.Parkings.FindAsync(parking.Id);
            if (existingParking == null)
                return null;

            _context.Entry(existingParking).CurrentValues.SetValues(parking);
            await _context.SaveChangesAsync();
            return existingParking;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var parking = await _context.Parkings.FindAsync(id);
            if (parking == null)
                return false;

            _context.Parkings.Remove(parking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
