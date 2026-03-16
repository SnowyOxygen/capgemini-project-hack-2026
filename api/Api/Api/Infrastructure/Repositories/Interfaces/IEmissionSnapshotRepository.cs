using Api.Models;

namespace Api.Infrastructure.Repositories.Interfaces
{
    public interface IEmissionSnapshotRepository
    {
        Task<IEnumerable<EmissionSnapshot>> GetAllAsync();
        Task<EmissionSnapshot?> GetByIdAsync(int id);
        Task<IEnumerable<EmissionSnapshot>> GetBySiteIdAsync(int siteId);
        Task<IEnumerable<EmissionSnapshot>> GetBySiteIdAndDateRangeAsync(int siteId, DateTime dateDebut, DateTime dateFin);
        Task<EmissionSnapshot?> GetLatestBySiteIdAsync(int siteId);
        Task<EmissionSnapshot> CreateAsync(EmissionSnapshot emissionSnapshot);
        Task<EmissionSnapshot?> UpdateAsync(EmissionSnapshot emissionSnapshot);
        Task<bool> DeleteAsync(int id);
    }
}
