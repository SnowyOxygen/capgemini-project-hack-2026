using Api.Models;

namespace Api.Infrastructure.Repositories.Interfaces
{
    public interface ISiteMateriauRepository
    {
        Task<IEnumerable<SiteMateriau>> GetAllAsync();
        Task<SiteMateriau?> GetByIdAsync(int id);
        Task<IEnumerable<SiteMateriau>> GetBySiteIdAsync(int siteId);
        Task<IEnumerable<SiteMateriau>> GetByMateriauIdAsync(int materiauId);
        Task<SiteMateriau> CreateAsync(SiteMateriau siteMateriau);
        Task<SiteMateriau?> UpdateAsync(SiteMateriau siteMateriau);
        Task<bool> DeleteAsync(int id);
    }
}
