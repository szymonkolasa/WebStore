using System.ComponentModel.DataAnnotations;

namespace WebStore.Web.ViewModels.Account
{
    public class ShippingInformationViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

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
    }
}
