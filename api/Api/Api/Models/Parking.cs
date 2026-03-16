using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    [Table("parkings")]
    public class Parking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SiteId { get; set; }

        public int? NombrePlacesTotal { get; set; }

        public int? PlacesAeriennes { get; set; }

        public int? PlacesSousDalle { get; set; }

        public int? PlacesSousSol { get; set; }

        [ForeignKey(nameof(SiteId))]
        public Site? Site { get; set; }
    }
}
