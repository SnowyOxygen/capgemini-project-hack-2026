using Api.DTOs;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing sites and their related data
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SitesController : ControllerBase
    {
        private readonly ISiteService _siteService;

        /// <summary>
        /// Initializes a new instance of the SitesController
        /// </summary>
        /// <param name="siteService">Site service for site operations</param>
        public SitesController(ISiteService siteService)
        {
            _siteService = siteService;
        }

        /// <summary>
        /// Creates a new site with all related entities (parking, energies, materials)
        /// </summary>
        /// <param name="request">Complete site creation request with all related data</param>
        /// <returns>The created site with all relationships</returns>
        /// <response code="201">Returns the newly created site</response>
        /// <response code="400">If the request is invalid or validation fails</response>
        /// <response code="401">If authentication is missing or invalid</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/sites
        ///     {
        ///       "site": {
        ///         "nom": "Tour Eiffel Office Complex",
        ///         "typeSite": "Bureau",
        ///         "anneeConstruction": 2018,
        ///         "superficieM2": 15000.50,
        ///         "nombreEtages": 12,
        ///         "nombrePersonnes": 450
        ///       },
        ///       "parking": {
        ///         "nombrePlacesTotal": 200,
        ///         "placesAeriennes": 50,
        ///         "placesSousDalle": 75,
        ///         "placesSousSol": 75
        ///       },
        ///       "energies": [
        ///         {
        ///           "typeEnergie": "Électricité",
        ///           "consommationAnnuelle": 850000.00,
        ///           "unite": "kWh",
        ///           "typeDonnee": "Réelle",
        ///           "periodeDebut": "2024-01-01T00:00:00Z",
        ///           "periodeFin": "2024-12-31T23:59:59Z"
        ///         }
        ///       ],
        ///       "materiaux": [
        ///         {
        ///           "materiauId": 1,
        ///           "quantite": 5000.00,
        ///           "unite": "t"
        ///         }
        ///       ]
        ///     }
        ///     
        /// Notes:
        /// - Only the site name (nom) is required
        /// - Parking, energies, and materials are optional
        /// - Materials must reference existing material IDs from the catalog
        /// - All data is saved in a single transaction
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(SiteResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SiteResponse>> CreateSite([FromBody] CreateSiteRequest request)
        {
            try
            {
                var site = await _siteService.CreateSiteAsync(request);
                return CreatedAtAction(nameof(CreateSite), new { id = site.Id }, site);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Une erreur est survenue lors de la création du site.", details = ex.Message });
            }
        }
    }
}
