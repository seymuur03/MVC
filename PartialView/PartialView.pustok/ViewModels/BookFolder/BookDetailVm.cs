using PartialView.pustok.Models;

namespace PartialView.pustok.ViewModels.BookFolder
{
    public class BookDetailVm
    {
        public Book Book { get; set; }
        public List<Book> RelatedBooks { get; set; }

    }
}
