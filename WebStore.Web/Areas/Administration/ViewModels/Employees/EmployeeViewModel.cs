using System;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Web.Areas.Administration.ViewModels.Employees
{
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Phone number field is required")]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "The Role field is required")]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
