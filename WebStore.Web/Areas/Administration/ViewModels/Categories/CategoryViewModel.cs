using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Web.Areas.Administration.ViewModels.Categories
{
    public class CategoryViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name field is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Image")]
        public IFormFile CategoryImage { get; set; }

        [Display(Name = "Parent category")]
        public String ParentCategory { get; set; }

        [Display(Name = "Show on bar")]
        public bool ShowOnBar { get; set; }
    }
}
