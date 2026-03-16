using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    [Table("energies")]
    public class Energie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SiteId { get; set; }

        [MaxLength(50)]
        public string? TypeEnergie { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal? ConsommationAnnuelle { get; set; }

        [MaxLength(20)]
        public string? Unite { get; set; }

        [MaxLength(20)]
        public string? TypeDonnee { get; set; }

        public DateTime? PeriodeDebut { get; set; }

        public DateTime? PeriodeFin { get; set; }

        [ForeignKey(nameof(SiteId))]
        public Site? Site { get; set; }
    }
}
