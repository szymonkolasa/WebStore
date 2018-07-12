using System.ComponentModel.DataAnnotations;

namespace WebStore.Web.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "The Email field is required")]
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Password field is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool Remember { get; set; }
    }
}
