using System;
using System.ComponentModel.DataAnnotations;
using WebStore.Data.Entities;

namespace WebStore.Web.Areas.Administration.ViewModels.Categories
{
    public class CategoriesViewModel
    {
        public Guid Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Show on bar")]
        public bool ShowOnBar { get; set; }

        [Display(Name = "Parent category")]
        public Category ParentCategory { get; set; }
    }
}
