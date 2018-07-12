using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebStore.Data.Entities;

namespace WebStore.Web.Areas.Administration.ViewModels.Products
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name field is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Shor description field is required")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Short description")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Description field is required")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price field is required")]
        [DataType(DataType.Currency, ErrorMessage = "Price field must be a number")]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Display(Name = "Thumbnail")]
        public IFormFile Thumbnail { get; set; }

        [Display(Name = "Images")]
        public List<IFormFile> Images { get; set; }

        [Required(ErrorMessage = "Category field is required")]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Product images")]
        public List<ProductImage> ProductImages { get; set; }
    }
}
