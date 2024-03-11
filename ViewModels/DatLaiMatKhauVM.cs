using System.ComponentModel.DataAnnotations;

namespace KLTN_E.ViewModels
{
    public class DatLaiMatKhauVM
    {
        [Required(ErrorMessage = "Old password is required.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "New password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
