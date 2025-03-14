using PartialView.pustok.Models;

namespace PartialView.pustok.ViewModels.Home
{
    public class HomeVm
    {
        public List<Slider> Sliders{ get; set; }
        public List<Feature> Features{ get; set; }
        public List<Book> FeaturedBooks { get; set; }
        public List<Book> NewBooks { get; set; }
        public List<Book> DiscBooks { get; set; }
    }
}
