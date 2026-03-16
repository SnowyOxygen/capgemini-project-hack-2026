using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    [Table("site_materiaux")]
    public class SiteMateriau
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SiteId { get; set; }

        [Required]
        public int MateriauId { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal? Quantite { get; set; }

        [MaxLength(10)]
        public string? Unite { get; set; }

        [ForeignKey(nameof(SiteId))]
        public Site? Site { get; set; }

        [ForeignKey(nameof(MateriauId))]
        public Materiau? Materiau { get; set; }
    }
}
