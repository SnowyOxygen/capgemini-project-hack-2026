using Api.Models;

namespace Api.Infrastructure.Repositories.Interfaces
{
    public interface IFacteurEnergieRepository
    {
        Task<IEnumerable<FacteurEnergie>> GetAllAsync();
        Task<FacteurEnergie?> GetByIdAsync(int id);
        Task<FacteurEnergie?> GetByTypeEnergieAsync(string typeEnergie);
        Task<FacteurEnergie> CreateAsync(FacteurEnergie facteurEnergie);
        Task<FacteurEnergie?> UpdateAsync(FacteurEnergie facteurEnergie);
        Task<bool> DeleteAsync(int id);
    }
}
