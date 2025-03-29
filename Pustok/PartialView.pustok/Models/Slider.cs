using PartialView.pustok.Areas.Manage.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LengthAttribute = PartialView.pustok.Areas.Manage.Attributes.LengthAttribute;

namespace PartialView.pustok.Models
{
    public class Slider : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Desc { get; set; }
        public string ImgName { get; set; }
        [Required]
        public string ButtonLink { get; set; }
        [Required]
        [StringLength(80)]
        public string ButtonText { get; set; }
        [Required]
        public int Order { get; set; }
        [NotMapped]
        [TypeAttribute("image/png","image/jpeg")]
        [LengthAttribute(2*1024*1024)]
        public IFormFile Photo { get; set; }
    }
}
