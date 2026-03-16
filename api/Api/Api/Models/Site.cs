using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    [Table("sites")]
    public class Site
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Nom { get; set; }

        [MaxLength(100)]
        public string? TypeSite { get; set; }

        public int? AnneeConstruction { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? SuperficieM2 { get; set; }

        public int? NombreEtages { get; set; }

        public int? NombrePersonnes { get; set; }

        public ICollection<Parking> Parkings { get; set; } = new List<Parking>();
        public ICollection<Energie> Energies { get; set; } = new List<Energie>();
        public ICollection<SiteMateriau> SiteMateriaux { get; set; } = new List<SiteMateriau>();
        public ICollection<EmissionMensuelle> EmissionsMensuelles { get; set; } = new List<EmissionMensuelle>();
        public ICollection<EmissionSnapshot> EmissionSnapshots { get; set; } = new List<EmissionSnapshot>();
    }
}
