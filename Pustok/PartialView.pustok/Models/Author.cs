using System.ComponentModel.DataAnnotations;

namespace PartialView.pustok.Models
{
    public class Author : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
