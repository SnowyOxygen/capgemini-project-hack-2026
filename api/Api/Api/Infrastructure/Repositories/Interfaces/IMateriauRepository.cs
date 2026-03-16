using Api.Models;

namespace Api.Infrastructure.Repositories.Interfaces
{
    public interface IMateriauRepository
    {
        Task<IEnumerable<Materiau>> GetAllAsync();
        Task<Materiau?> GetByIdAsync(int id);
        Task<Materiau> CreateAsync(Materiau materiau);
        Task<Materiau?> UpdateAsync(Materiau materiau);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
