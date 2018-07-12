using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Web.Areas.Administration.ViewModels.Banners
{
    public class BannerViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The Name field is required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Start date field is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "The End date field is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "The Link field is required")]
        [Display(Name = "Link")]
        public string Link { get; set; }

        [Display(Name = "File")]
        public IFormFile File { get; set; }
    }
}
