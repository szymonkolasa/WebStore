using System.Collections.Generic;

namespace WebStore.Web.ViewModels.Product
{
    public class ProductViewModel
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public Data.Entities.Category Category { get; set; }
        public List<Data.Entities.Product> Products { get; set; }
    }
}
