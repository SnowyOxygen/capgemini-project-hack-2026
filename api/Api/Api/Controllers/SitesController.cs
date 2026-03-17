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
        /// Retrieves all sites with their complete data
        /// </summary>
        /// <returns>List of all sites with parking, energies, and materials</returns>
        /// <response code="200">Returns the list of all sites</response>
        /// <response code="401">If authentication is missing or invalid</response>
        /// <remarks>
        /// Sample response:
        /// 
        ///     GET /api/sites
        ///     [
        ///       {
        ///         "id": 1,
        ///         "nom": "Tour Eiffel Office Complex",
        ///         "typeSite": "Bureau",
        ///         "anneeConstruction": 2018,
        ///         "superficieM2": 15000.50,
        ///         "nombreEtages": 12,
        ///         "nombrePersonnes": 450,
        ///         "parking": {
        ///           "id": 1,
        ///           "nombrePlacesTotal": 200,
        ///           "placesAeriennes": 50,
        ///           "placesSousDalle": 75,
        ///           "placesSousSol": 75
        ///         },
        ///         "energies": [
        ///           {
        ///             "id": 1,
        ///             "typeEnergie": "Électricité",
        ///             "consommationAnnuelle": 850000.00,
        ///             "unite": "kWh",
        ///             "typeDonnee": "Réelle",
        ///             "periodeDebut": "2024-01-01T00:00:00Z",
        ///             "periodeFin": "2024-12-31T23:59:59Z"
        ///           }
        ///         ],
        ///         "materiaux": [
        ///           {
        ///             "id": 1,
        ///             "materiauId": 1,
        ///             "materiauNom": "Béton",
        ///             "quantite": 5000.00,
        ///             "unite": "t"
        ///           }
        ///         ],
        ///         "createdAt": "2024-01-15T10:30:00Z"
        ///       }
        ///     ]
        ///     
        /// Each site includes:
        /// - All basic site information
        /// - Parking details (if exists)
        /// - All energy consumption records
        /// - All materials with quantities and names
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SiteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<SiteResponse>>> GetAllSites()
        {
            try
            {
                var sites = await _siteService.GetAllSitesAsync();
                return Ok(sites);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Une erreur est survenue lors de la récupération des sites.", details = ex.Message });
            }
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

        /// <summary>
        /// Updates basic site information (partial update)
        /// </summary>
        /// <param name="id">Site ID</param>
        /// <param name="request">Fields to update (all fields are optional)</param>
        /// <returns>The updated site with all relationships</returns>
        /// <response code="200">Returns the updated site</response>
        /// <response code="400">If validation fails</response>
        /// <response code="401">If authentication is missing or invalid</response>
        /// <response code="404">If the site does not exist</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH /api/sites/1
        ///     {
        ///       "nom": "Updated Site Name",
        ///       "nombrePersonnes": 500
        ///     }
        ///     
        /// Only include fields you want to update. All fields are optional.
        /// </remarks>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(SiteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SiteResponse>> UpdateSite(int id, [FromBody] UpdateSiteRequest request)
        {
            try
            {
                var site = await _siteService.UpdateSiteAsync(id, request);
                return Ok(site);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Une erreur est survenue lors de la mise à jour du site.", details = ex.Message });
            }
        }

        /// <summary>
        /// Creates or updates parking information for a site
        /// </summary>
        /// <param name="id">Site ID</param>
        /// <param name="request">Parking information</param>
        /// <returns>The parking information</returns>
        /// <response code="200">Returns the parking information</response>
        /// <response code="400">If validation fails</response>
        /// <response code="401">If authentication is missing or invalid</response>
        /// <response code="404">If the site does not exist</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/sites/1/parking
        ///     {
        ///       "nombrePlacesTotal": 200,
        ///       "placesAeriennes": 50,
        ///       "placesSousDalle": 75,
        ///       "placesSousSol": 75
        ///     }
        ///     
        /// If parking exists, it will be updated. If not, it will be created.
        /// </remarks>
        [HttpPut("{id}/parking")]
        [ProducesResponseType(typeof(ParkingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ParkingResponse>> UpsertParking(int id, [FromBody] UpsertParkingRequest request)
        {
            try
            {
                var parking = await _siteService.UpsertParkingAsync(id, request);
                return Ok(parking);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Une erreur est survenue.", details = ex.Message });
            }
        }

        /// <summary>
        /// Deletes parking information for a site
        /// </summary>
        /// <param name="id">Site ID</param>
        /// <returns>Success status</returns>
        /// <response code="204">Parking deleted successfully</response>
        /// <response code="401">If authentication is missing or invalid</response>
        /// <response code="404">If parking does not exist for this site</response>
        [HttpDelete("{id}/parking")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteParking(int id)
        {
            var deleted = await _siteService.DeleteParkingAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = $"Aucun parking trouvé pour le site {id}." });
            }
            return NoContent();
        }

        /// <summary>
        /// Retrieves the energy consumption history for a specific site
        /// </summary>
        /// <param name="id">Site ID</param>
        /// <returns>List of energy consumption records ordered by most recent first</returns>
        /// <response code="200">Returns the list of energy consumption records</response>
        /// <response code="401">If authentication is missing or invalid</response>
        /// <response code="404">If the site does not exist</response>
        /// <remarks>
        /// Sample response:
        /// 
        ///     GET /api/sites/1/energies
        ///     [
        ///       {
        ///         "id": 1,
        ///         "typeEnergie": "Électricité",
        ///         "consommationAnnuelle": 850000.00,
        ///         "unite": "kWh",
        ///         "typeDonnee": "Réelle",
        ///         "periodeDebut": "2024-01-01T00:00:00Z",
        ///         "periodeFin": "2024-12-31T23:59:59Z"
        ///       },
        ///       {
        ///         "id": 2,
        ///         "typeEnergie": "Gaz",
        ///         "consommationAnnuelle": 125000.00,
        ///         "unite": "kWh",
        ///         "typeDonnee": "Estimée",
        ///         "periodeDebut": "2024-01-01T00:00:00Z",
        ///         "periodeFin": "2024-12-31T23:59:59Z"
        ///       }
        ///     ]
        ///     
        /// Energy records are ordered by period start date (most recent first), 
        /// then by ID for records without dates.
        /// </remarks>
        [HttpGet("{id}/energies")]
        [ProducesResponseType(typeof(IEnumerable<EnergieResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EnergieResponse>>> GetEnergies(int id)
        {
            try
            {
                var energies = await _siteService.GetEnergiesForSiteAsync(id);
                return Ok(energies);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Une erreur est survenue lors de la récupération des énergies.", details = ex.Message });
            }
        }

        /// <summary>
        /// Adds a new energy consumption record to a site
        /// </summary>
        /// <param name="id">Site ID</param>
        /// <param name="request">Energy consumption data</param>
        /// <returns>The created energy record</returns>
        /// <response code="201">Returns the newly created energy record</response>
        /// <response code="400">If validation fails</response>
        /// <response code="401">If authentication is missing or invalid</response>
        /// <response code="404">If the site does not exist</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/sites/1/energies
        ///     {
        ///       "typeEnergie": "Électricité",
        ///       "consommationAnnuelle": 850000.00,
        ///       "unite": "kWh",
        ///       "typeDonnee": "Réelle",
        ///       "periodeDebut": "2024-01-01T00:00:00Z",
        ///       "periodeFin": "2024-12-31T23:59:59Z"
        ///     }
        /// </remarks>
        [HttpPost("{id}/energies")]
        [ProducesResponseType(typeof(EnergieResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EnergieResponse>> AddEnergie(int id, [FromBody] AddEnergieRequest request)
        {
            try
            {
                var energie = await _siteService.AddEnergieAsync(id, request);
                return CreatedAtAction(nameof(AddEnergie), new { id, energieId = energie.Id }, energie);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Une erreur est survenue.", details = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing energy consumption record
        /// </summary>
        /// <param name="id">Site ID</param>
        /// <param name="energieId">Energy record ID</param>
        /// <param name="request">Updated energy data</param>
        /// <returns>The updated energy record</returns>
        /// <response code="200">Returns the updated energy record</response>
        /// <response code="400">If validation fails</response>
        /// <response code="401">If authentication is missing or invalid</response>
        /// <response code="404">If the site or energy record does not exist</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/sites/1/energies/5
        ///     {
        ///       "consommationAnnuelle": 900000.00,
        ///       "typeDonnee": "Réelle"
        ///     }
        ///     
        /// Only include fields you want to update.
        /// </remarks>
        [HttpPut("{id}/energies/{energieId}")]
        [ProducesResponseType(typeof(EnergieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EnergieResponse>> UpdateEnergie(int id, int energieId, [FromBody] UpdateEnergieRequest request)
        {
            try
            {
                var energie = await _siteService.UpdateEnergieAsync(id, energieId, request);
                return Ok(energie);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Une erreur est survenue.", details = ex.Message });
            }
        }

        /// <summary>
        /// Deletes an energy consumption record from a site
        /// </summary>
        /// <param name="id">Site ID</param>
        /// <param name="energieId">Energy record ID</param>
        /// <returns>Success status</returns>
        /// <response code="204">Energy record deleted successfully</response>
        /// <response code="401">If authentication is missing or invalid</response>
        /// <response code="404">If the energy record does not exist</response>
        [HttpDelete("{id}/energies/{energieId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEnergie(int id, int energieId)
        {
            var deleted = await _siteService.DeleteEnergieAsync(id, energieId);
            if (!deleted)
            {
                return NotFound(new { message = $"Énergie {energieId} non trouvée pour le site {id}." });
            }
            return NoContent();
        }

        /// <summary>
        /// Adds a material to a site
        /// </summary>
        /// <param name="id">Site ID</param>
        /// <param name="request">Material data with quantity and unit</param>
        /// <returns>The created material relationship</returns>
        /// <response code="201">Returns the newly created material relationship</response>
        /// <response code="400">If validation fails or material already exists</response>
        /// <response code="401">If authentication is missing or invalid</response>
        /// <response code="404">If the site or material does not exist</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/sites/1/materiaux
        ///     {
        ///       "materiauId": 1,
        ///       "quantite": 5000.00,
        ///       "unite": "t"
        ///     }
        ///     
        /// The material must exist in the materials catalog.
        /// Cannot add the same material twice to a site.
        /// </remarks>
        [HttpPost("{id}/materiaux")]
        [ProducesResponseType(typeof(SiteMateriauResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SiteMateriauResponse>> AddMateriau(int id, [FromBody] AddSiteMateriauRequest request)
        {
            try
            {
                var materiau = await _siteService.AddMateriauAsync(id, request);
                return CreatedAtAction(nameof(AddMateriau), new { id, materiauId = materiau.Id }, materiau);
            }
            catch (InvalidOperationException ex)
            {
                var message = ex.Message;
                if (message.Contains("n'existe pas"))
                {
                    return NotFound(new { message });
                }
                return BadRequest(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Une erreur est survenue.", details = ex.Message });
            }
        }

        /// <summary>
        /// Updates material quantity and unit for a site
        /// </summary>
        /// <param name="id">Site ID</param>
        /// <param name="siteMateriauId">Site-Material relationship ID</param>
        /// <param name="request">Updated quantity and/or unit</param>
        /// <returns>The updated material relationship</returns>
        /// <response code="200">Returns the updated material relationship</response>
        /// <response code="400">If validation fails</response>
        /// <response code="401">If authentication is missing or invalid</response>
        /// <response code="404">If the relationship does not exist</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/sites/1/materiaux/3
        ///     {
        ///       "quantite": 6000.00
        ///     }
        ///     
        /// Only include fields you want to update.
        /// Cannot change the material ID, only quantity and unit.
        /// </remarks>
        [HttpPut("{id}/materiaux/{siteMateriauId}")]
        [ProducesResponseType(typeof(SiteMateriauResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SiteMateriauResponse>> UpdateMateriau(int id, int siteMateriauId, [FromBody] UpdateSiteMateriauRequest request)
        {
            try
            {
                var materiau = await _siteService.UpdateMateriauAsync(id, siteMateriauId, request);
                return Ok(materiau);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Une erreur est survenue.", details = ex.Message });
            }
        }

        /// <summary>
        /// Removes a material from a site
        /// </summary>
        /// <param name="id">Site ID</param>
        /// <param name="siteMateriauId">Site-Material relationship ID</param>
        /// <returns>Success status</returns>
        /// <response code="204">Material relationship deleted successfully</response>
        /// <response code="401">If authentication is missing or invalid</response>
        /// <response code="404">If the relationship does not exist</response>
        [HttpDelete("{id}/materiaux/{siteMateriauId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMateriau(int id, int siteMateriauId)
        {
            var deleted = await _siteService.DeleteMateriauAsync(id, siteMateriauId);
            if (!deleted)
            {
                return NotFound(new { message = $"Matériau {siteMateriauId} non trouvé pour le site {id}." });
            }
            return NoContent();
        }
    }
}
