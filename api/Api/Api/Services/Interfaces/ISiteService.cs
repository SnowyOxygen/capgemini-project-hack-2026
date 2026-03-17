using Api.DTOs;

namespace Api.Services.Interfaces
{
    public interface ISiteService
    {
        Task<IEnumerable<SiteResponse>> GetAllSitesAsync();
        Task<SiteResponse> CreateSiteAsync(CreateSiteRequest request);
        Task<SiteResponse> UpdateSiteAsync(int siteId, UpdateSiteRequest request);
        Task<ParkingResponse> UpsertParkingAsync(int siteId, UpsertParkingRequest request);
        Task<bool> DeleteParkingAsync(int siteId);
        Task<IEnumerable<EnergieResponse>> GetEnergiesForSiteAsync(int siteId);
        Task<EnergieResponse> AddEnergieAsync(int siteId, AddEnergieRequest request);
        Task<EnergieResponse> UpdateEnergieAsync(int siteId, int energieId, UpdateEnergieRequest request);
        Task<bool> DeleteEnergieAsync(int siteId, int energieId);
        Task<SiteMateriauResponse> AddMateriauAsync(int siteId, AddSiteMateriauRequest request);
        Task<SiteMateriauResponse> UpdateMateriauAsync(int siteId, int siteMateriauId, UpdateSiteMateriauRequest request);
        Task<bool> DeleteMateriauAsync(int siteId, int siteMateriauId);
    }
}
