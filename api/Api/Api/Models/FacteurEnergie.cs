using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    [Table("facteurs_energie")]
    public class FacteurEnergie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? TypeEnergie { get; set; }

        [Column(TypeName = "decimal(10,5)")]
        public decimal? FacteurEmission { get; set; }

        [MaxLength(20)]
        public string? Unite { get; set; }
    }
}
