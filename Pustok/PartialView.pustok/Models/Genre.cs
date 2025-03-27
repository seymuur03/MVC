using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PartialView.pustok.Models
{
    public class Genre:BaseEntity
    {
        
        [Required]
        public string Name { get; set; }
       
        public List<Book> Books { get; set; }
    }
}
