using System.Collections.Generic;
using System.Linq;
using WebStore.Data.Entities;

namespace WebStore.Web.Models.Cart
{
    public class Cart
    {
        private List<CartItem> _items;

        public IEnumerable<CartItem> Items => _items;
        public double Total => _items.Sum(x => x.Product.Price * x.Quantity);
        public int Count => _items.Sum(x => x.Quantity);

        public Cart()
        {
            _items = new List<CartItem>();
        }

        public void Add(Product product, int quantity = 1)
        {
            var item = _items
                .FirstOrDefault(x => x.Product.Id == product.Id);

            if (item is null)
            {
                _items.Add(new CartItem
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                if ((item.Quantity + quantity) > 0)
                {
                    item.Quantity += quantity;
                }
                else
                {
                    Remove(product);
                }
            }
        }

        public void Remove(Product product) => _items.RemoveAll(x => x.Product.Id == product.Id);
        
        public void Clear() => _items.Clear();
    }
}
