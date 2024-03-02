using System.ComponentModel.DataAnnotations;

namespace KLTN_E.ViewModels
{
    public class RegisterVM
    {
        [Key]
        [Display(Name = "UserName")]
        [Required(ErrorMessage = "The UserName is required")]
        [MaxLength(20, ErrorMessage = "Maximum 20 characters")]
        public string MaKh { get; set; }

        [Display(Name = "Password")]

        [Required(ErrorMessage = "The Password is required")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Display(Name = "FullName")]

        [Required(ErrorMessage = "The FullName is required")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters")]
        public string HoTen { get; set; }

        [Display(Name = "DateOfBirth")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [MaxLength(60, ErrorMessage = "Maximum 60 characters")]
        [Display(Name = "Address")]
        [Required(ErrorMessage = "The Address is required")]
        public string DiaChi { get; set; }

        [MaxLength(24, ErrorMessage = "Maximum 24 characters")]
        [RegularExpression(@"0[9875]\d{8}", ErrorMessage = "Please enter a valid Phone Number")]
        [Display(Name = "Phone")]
        [Required(ErrorMessage = "The Phone is required")]
        public string DienThoai { get; set; }
        [EmailAddress(ErrorMessage = "Please enter a valid Email Address")]
        [Required(ErrorMessage = "The Email is required")]
        public string Email { get; set; }

        public string? Hinh { get; set; }
    }
}
