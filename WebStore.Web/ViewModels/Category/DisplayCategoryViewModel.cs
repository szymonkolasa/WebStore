using System.Collections.Generic;

namespace WebStore.Web.ViewModels.Category
{
    public class DisplayCategoryViewModel
    {
        public Data.Entities.Category ParentCategory { get; set; }
        public IEnumerable<Data.Entities.Category> Categories { get; set; }
    }
}
