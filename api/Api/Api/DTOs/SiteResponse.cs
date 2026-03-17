namespace Api.DTOs
{
    /// <summary>
    /// Response model for a created site
    /// </summary>
    public class SiteResponse
    {
        /// <summary>
        /// Site ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the site
        /// </summary>
        public required string Nom { get; set; }

        /// <summary>
        /// Type/category of the site
        /// </summary>
        public string? TypeSite { get; set; }

        /// <summary>
        /// Year the site was constructed
        /// </summary>
        public int? AnneeConstruction { get; set; }

        /// <summary>
        /// Total surface area in square meters
        /// </summary>
        public decimal? SuperficieM2 { get; set; }

        /// <summary>
        /// Number of floors/levels
        /// </summary>
        public int? NombreEtages { get; set; }

        /// <summary>
        /// Number of people occupying the site
        /// </summary>
        public int? NombrePersonnes { get; set; }

        /// <summary>
        /// Parking information
        /// </summary>
        public ParkingResponse? Parking { get; set; }

        /// <summary>
        /// Energy consumption records
        /// </summary>
        public List<EnergieResponse> Energies { get; set; } = new();

        /// <summary>
        /// Construction materials used
        /// </summary>
        public List<SiteMateriauResponse> Materiaux { get; set; } = new();

        /// <summary>
        /// Total construction emissions (calculated)
        /// </summary>
        public decimal EmissionsConstruction { get; set; }

        /// <summary>
        /// Total annual emissions (calculated)
        /// </summary>
        public decimal EmissionsAnnuelles { get; set; }

        /// <summary>
        /// Timestamp when the site was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Parking response model
    /// </summary>
    public class ParkingResponse
    {
        public int Id { get; set; }
        public int? NombrePlacesTotal { get; set; }
        public int? PlacesAeriennes { get; set; }
        public int? PlacesSousDalle { get; set; }
        public int? PlacesSousSol { get; set; }
    }

    /// <summary>
    /// Energy response model
    /// </summary>
    public class EnergieResponse
    {
        public int Id { get; set; }
        public string? TypeEnergie { get; set; }
        public decimal? ConsommationAnnuelle { get; set; }
        public string? Unite { get; set; }
        public string? TypeDonnee { get; set; }
        public DateTime? PeriodeDebut { get; set; }
        public DateTime? PeriodeFin { get; set; }
    }

    /// <summary>
    /// Material response model
    /// </summary>
    public class SiteMateriauResponse
    {
        public int Id { get; set; }
        public int MateriauId { get; set; }
        public string MateriauNom { get; set; } = string.Empty;
        public decimal? Quantite { get; set; }
        public string? Unite { get; set; }
    }
}
