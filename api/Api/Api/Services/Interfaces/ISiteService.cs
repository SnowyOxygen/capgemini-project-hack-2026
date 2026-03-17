using Api.DTOs;

namespace Api.Services.Interfaces
{
    public interface ISiteService
    {
        Task<SiteResponse> CreateSiteAsync(CreateSiteRequest request);
    }
}
