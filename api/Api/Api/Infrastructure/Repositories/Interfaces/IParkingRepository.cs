using Api.Models;

namespace Api.Infrastructure.Repositories.Interfaces
{
    public interface IParkingRepository
    {
        Task<IEnumerable<Parking>> GetAllAsync();
        Task<Parking?> GetByIdAsync(int id);
        Task<IEnumerable<Parking>> GetBySiteIdAsync(int siteId);
        Task<Parking> CreateAsync(Parking parking);
        Task<Parking?> UpdateAsync(Parking parking);
        Task<bool> DeleteAsync(int id);
    }
}
