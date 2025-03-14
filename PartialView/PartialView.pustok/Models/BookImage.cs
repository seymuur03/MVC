using System.ComponentModel.DataAnnotations;

namespace PartialView.pustok.Models
{
    public class BookImage
    {
        public int Id { get; set; }
        [Required]
        public string ImgName { get; set; }
        public bool? Status { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
