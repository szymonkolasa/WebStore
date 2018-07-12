using WebStore.Data.Entities;

namespace WebStore.Web.Models.Cart
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
