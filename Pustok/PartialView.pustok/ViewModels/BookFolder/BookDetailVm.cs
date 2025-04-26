using PartialView.pustok.Models;

namespace PartialView.pustok.ViewModels.BookFolder
{
    public class BookDetailVm
    {
        public Book Book { get; set; }
        public BookComment BookComment { get; set; }
        public List<Book> RelatedBooks { get; set; }

        public bool HasComment { get; set; }
        public int? TotalComments { get; set; }
        public decimal? AverageRate { get; set; }
    }
}
