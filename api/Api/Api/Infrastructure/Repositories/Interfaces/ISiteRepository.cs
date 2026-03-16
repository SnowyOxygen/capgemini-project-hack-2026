using Api.Models;

namespace Api.Infrastructure.Repositories.Interfaces
{
    public interface ISiteRepository
    {
        Task<IEnumerable<Site>> GetAllAsync();
        Task<Site?> GetByIdAsync(int id);
        Task<Site> CreateAsync(Site site);
        Task<Site?> UpdateAsync(Site site);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
