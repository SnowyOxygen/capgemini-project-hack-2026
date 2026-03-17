using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    /// <summary>
    /// Request model for creating a new site with all related entities
    /// </summary>
    public class CreateSiteRequest
    {
        /// <summary>
        /// Site core information
        /// </summary>
        [Required]
        public required SiteDto Site { get; set; }

        /// <summary>
        /// Parking information (optional)
        /// </summary>
        public ParkingDto? Parking { get; set; }

        /// <summary>
        /// Energy consumption records (optional)
        /// </summary>
        public List<EnergieDto>? Energies { get; set; }

        /// <summary>
        /// Construction materials used (optional)
        /// </summary>
        public List<SiteMateriauDto>? Materiaux { get; set; }
    }

    /// <summary>
    /// Site core information
    /// </summary>
    public class SiteDto
    {
        /// <summary>
        /// Name of the site
        /// </summary>
        /// <example>Tour Eiffel Office Complex</example>
        [Required]
        [MaxLength(255)]
        public required string Nom { get; set; }

        /// <summary>
        /// Type/category of the site
        /// </summary>
        /// <example>Bureau</example>
        [MaxLength(100)]
        public string? TypeSite { get; set; }

        /// <summary>
        /// Year the site was constructed
        /// </summary>
        /// <example>2018</example>
        [Range(1800, 2100)]
        public int? AnneeConstruction { get; set; }

        /// <summary>
        /// Total surface area in square meters
        /// </summary>
        /// <example>15000.50</example>
        [Range(0.01, 999999999999.99)]
        public decimal? SuperficieM2 { get; set; }

        /// <summary>
        /// Number of floors/levels
        /// </summary>
        /// <example>12</example>
        [Range(1, 1000)]
        public int? NombreEtages { get; set; }

        /// <summary>
        /// Number of people occupying the site
        /// </summary>
        /// <example>450</example>
        [Range(1, 1000000)]
        public int? NombrePersonnes { get; set; }
    }

    /// <summary>
    /// Parking information
    /// </summary>
    public class ParkingDto
    {
        /// <summary>
        /// Total number of parking spaces
        /// </summary>
        /// <example>200</example>
        [Range(0, 100000)]
        public int? NombrePlacesTotal { get; set; }

        /// <summary>
        /// Number of above-ground/outdoor parking spaces
        /// </summary>
        /// <example>50</example>
        [Range(0, 100000)]
        public int? PlacesAeriennes { get; set; }

        /// <summary>
        /// Number of parking spaces under concrete slab
        /// </summary>
        /// <example>75</example>
        [Range(0, 100000)]
        public int? PlacesSousDalle { get; set; }

        /// <summary>
        /// Number of underground parking spaces
        /// </summary>
        /// <example>75</example>
        [Range(0, 100000)]
        public int? PlacesSousSol { get; set; }
    }

    /// <summary>
    /// Energy consumption record
    /// </summary>
    public class EnergieDto
    {
        /// <summary>
        /// Type of energy
        /// </summary>
        /// <example>Électricité</example>
        [MaxLength(50)]
        public string? TypeEnergie { get; set; }

        /// <summary>
        /// Annual consumption amount
        /// </summary>
        /// <example>850000.00</example>
        [Range(0.01, 99999999999999.99)]
        public decimal? ConsommationAnnuelle { get; set; }

        /// <summary>
        /// Unit of measurement
        /// </summary>
        /// <example>kWh</example>
        [MaxLength(20)]
        public string? Unite { get; set; }

        /// <summary>
        /// Type of data (Estimée, Réelle, Calculée)
        /// </summary>
        /// <example>Réelle</example>
        [MaxLength(20)]
        public string? TypeDonnee { get; set; }

        /// <summary>
        /// Start date of the measurement period
        /// </summary>
        /// <example>2024-01-01T00:00:00Z</example>
        public DateTime? PeriodeDebut { get; set; }

        /// <summary>
        /// End date of the measurement period
        /// </summary>
        /// <example>2024-12-31T23:59:59Z</example>
        public DateTime? PeriodeFin { get; set; }
    }

    /// <summary>
    /// Material usage record
    /// </summary>
    public class SiteMateriauDto
    {
        /// <summary>
        /// ID of the material from the materials catalog
        /// </summary>
        /// <example>1</example>
        [Required]
        [Range(1, int.MaxValue)]
        public int MateriauId { get; set; }

        /// <summary>
        /// Quantity of material used
        /// </summary>
        /// <example>5000.00</example>
        [Range(0.01, 99999999999999.99)]
        public decimal? Quantite { get; set; }

        /// <summary>
        /// Unit of quantity (kg, t, m³, m²)
        /// </summary>
        /// <example>t</example>
        [MaxLength(10)]
        public string? Unite { get; set; }
    }
}
