using System.ComponentModel.DataAnnotations;

namespace PartialView.pustok.Models
{
    public class BookComment:BaseEntity
    {
        public string Text { get; set; }
        public decimal Rate { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public CommentStatus CommentStatus { get; set; } = CommentStatus.Pending;
    }

    public enum CommentStatus {
        Pending,
        Approved,
        Rejected

    }

}
