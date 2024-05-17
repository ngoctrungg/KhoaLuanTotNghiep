using PagedList;
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

        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Full Name")]

        public string FullName { get; set; }
        [Display(Name = "Address")]

        public string Address { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        [Display(Name = "Image")]

        public string ProfileImage { get; set; }
        public IPagedList<PurchaseHistoryVM>? PurchaseHistory { get; set;}

        [Display(Name = "Production Date")]

        public DateTime NgayDat { get; set; }
        [Display(Name = "Status")]

        public string TenTrangThai { get; set; }
        [Display(Name = "Product")]

        public string TenHangHoa { get; set; }
        [Display(Name = "Quantity")]

        public double SoLuong { get; set; }
        [Display(Name = "Price")]

        public double? DonGia { get; set; }
        [Display(Name = "Image")]

        public string? Hinh { get; set; }


    }
}
