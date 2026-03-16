using Api.Models;

namespace Api.Infrastructure.Repositories.Interfaces
{
    public interface IEnergieRepository
    {
        Task<IEnumerable<Energie>> GetAllAsync();
        Task<Energie?> GetByIdAsync(int id);
        Task<IEnumerable<Energie>> GetBySiteIdAsync(int siteId);
        Task<Energie> CreateAsync(Energie energie);
        Task<Energie?> UpdateAsync(Energie energie);
        Task<bool> DeleteAsync(int id);
    }
}
