using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    /// <summary>
    /// Request model for updating basic site information
    /// </summary>
    public class UpdateSiteRequest
    {
        /// <summary>
        /// Name of the site
        /// </summary>
        /// <example>Updated Office Complex</example>
        [MaxLength(255)]
        public string? Nom { get; set; }

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
    /// Request model for updating or creating parking information
    /// </summary>
    public class UpsertParkingRequest
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
    /// Request model for adding an energy consumption record
    /// </summary>
    public class AddEnergieRequest
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
    /// Request model for updating an energy consumption record
    /// </summary>
    public class UpdateEnergieRequest
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
    /// Request model for adding a material to a site
    /// </summary>
    public class AddSiteMateriauRequest
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

    /// <summary>
    /// Request model for updating a material record for a site
    /// </summary>
    public class UpdateSiteMateriauRequest
    {
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
