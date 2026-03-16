using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    [Table("materiaux")]
    public class Materiau
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Nom { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal? FacteurEmission { get; set; }

        public ICollection<SiteMateriau> SiteMateriaux { get; set; } = new List<SiteMateriau>();
    }
}
