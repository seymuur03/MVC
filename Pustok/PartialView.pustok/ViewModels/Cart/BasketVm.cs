using PartialView.pustok.Models;

namespace PartialView.pustok.ViewModels.Cart
{
    public class BasketVm
    {
       
        public int BookId { get; set; }
        public decimal Discount { get; set; }
        public string MainImage { get; set; }   
        public string ProdcutName { get; set; }   
        public decimal Price { get; set; }   
        public int Quantity { get; set; }   
        public decimal TotalPrice { get; set; }   
    }
}
