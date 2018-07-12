

using System.ComponentModel.DataAnnotations;

namespace WebStore.Web.ViewModels.Account
{
    public class EmailViewModel
    {
        [Required(ErrorMessage = "The Current email field is required")]
        [EmailAddress(ErrorMessage = "The Current email field is not a valid e-mail address")]
        [Display(Name = "Current email")]
        public string CurrentEmail { get; set; }

        [Required(ErrorMessage = "The New email field is required")]
        [EmailAddress(ErrorMessage = "The New email field is not a valid e-mail address")]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }

        [Required(ErrorMessage = "The Password field is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
