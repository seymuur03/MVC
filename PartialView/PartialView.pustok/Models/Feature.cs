using System.ComponentModel.DataAnnotations;

namespace PartialView.pustok.Models
{
    public class Feature
    {
        public int Id{ get; set; }
        [Required]
        [MaxLength(150)]
        public string Name{ get; set; }
        [Required]
        public string Description{ get; set; }
        public string IconName{ get; set; }
    }
}
