using PartialView.pustok.Areas.Manage.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LengthAttribute = PartialView.pustok.Areas.Manage.Attributes.LengthAttribute;

namespace PartialView.pustok.Models
{
    public class Book : BaseEntity
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string PCode { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }
        public bool InStock { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        [Required]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }
        public int Rate { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public List<BookImage> BookImages { get; set; } = new();
        public List<BookTag> BookTags { get; set; }
        [NotMapped]
        [TypeAttribute("image/png", "image/jpeg")]
        [LengthAttribute(2 * 1024 * 1024)]
        public List<IFormFile>? Photos { get; set; }
        [NotMapped]
        [TypeAttribute("image/png", "image/jpeg")]
        [LengthAttribute(2 * 1024 * 1024)]
        public IFormFile? MainPhoto { get; set; }
        [NotMapped]
        [TypeAttribute("image/png", "image/jpeg")]
        [LengthAttribute(2 * 1024 * 1024)]
        public IFormFile? HoverPhoto { get; set; }
        [NotMapped]
        public List<int>? ImgIds { get; set; }
        public List<BookComment> BookComments { get; set; }
        public Book()
        {
            BookComments = new();
        }

    }
}
