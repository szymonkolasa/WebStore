using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebStore.Web.ViewModels.Cart
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "The Last Name field is required.")]
        [DataType(DataType.Text)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The First name field is required.")]
        [DataType(DataType.Text)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The Street field is required.")]
        [DataType(DataType.Text)]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "The House number field is required.")]
        [DataType(DataType.Text)]
        [Display(Name = "House number")]
        public string HouseNumber { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Flat number")]
        public string FlatNumber { get; set; }

        [Required(ErrorMessage = "The City field is required.")]
        [DataType(DataType.Text)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "The Zip field is required.")]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Zip")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "The Country field is required.")]
        [DataType(DataType.Text)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "The Phone number field is required.")]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "This is a gift")]
        public bool IsGift { get; set; }
    }
}
