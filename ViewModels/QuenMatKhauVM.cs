using System.ComponentModel.DataAnnotations;

namespace KLTN_E.ViewModels
{
    public class QuenMatKhauVM
    {
        [Display(Name = "User Name")]
        public string MaKh { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }
    }
}
