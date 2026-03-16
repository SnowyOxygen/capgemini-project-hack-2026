using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    [Table("emission_snapshots")]
    public class EmissionSnapshot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SiteId { get; set; }

        [Required]
        public DateTime PeriodeDebut { get; set; }

        [Required]
        public DateTime PeriodeFin { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal? EmissionConstruction { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal? EmissionEnergieElectrique { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal? EmissionEnergieGaz { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal? EmissionTotale { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal? EmissionAnnuelle { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? EmissionParM2 { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? EmissionParPersonne { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(SiteId))]
        public Site? Site { get; set; }
    }
}
