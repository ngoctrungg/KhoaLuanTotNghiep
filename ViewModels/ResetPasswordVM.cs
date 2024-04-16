using System.ComponentModel.DataAnnotations;

namespace KLTN_E.ViewModels
{
    public class ResetPasswordVM
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string NewPassword { get; set; }
        [Compare("NewPassword", ErrorMessage = "Password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
