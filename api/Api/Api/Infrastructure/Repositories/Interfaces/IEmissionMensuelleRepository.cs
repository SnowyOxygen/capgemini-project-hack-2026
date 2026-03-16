using Api.Models;

namespace Api.Infrastructure.Repositories.Interfaces
{
    public interface IEmissionMensuelleRepository
    {
        Task<IEnumerable<EmissionMensuelle>> GetAllAsync();
        Task<EmissionMensuelle?> GetByIdAsync(int id);
        Task<IEnumerable<EmissionMensuelle>> GetBySiteIdAsync(int siteId);
        Task<IEnumerable<EmissionMensuelle>> GetBySiteIdAndYearAsync(int siteId, int annee);
        Task<IEnumerable<EmissionMensuelle>> GetBySiteIdAndDateRangeAsync(int siteId, DateTime dateDebut, DateTime dateFin);
        Task<IEnumerable<EmissionMensuelle>> GetLast12MonthsBySiteIdAsync(int siteId);
        Task<EmissionMensuelle?> GetBySiteIdYearMonthAsync(int siteId, int annee, int mois);
        Task<EmissionMensuelle> CreateAsync(EmissionMensuelle emissionMensuelle);
        Task<EmissionMensuelle?> UpdateAsync(EmissionMensuelle emissionMensuelle);
        Task<bool> DeleteAsync(int id);
    }
}
