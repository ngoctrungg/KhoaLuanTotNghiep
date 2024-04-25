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


        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public IPagedList<PurchaseHistoryVM>? PurchaseHistory { get; set;}


        public DateTime NgayDat { get; set; }
        public string TenTrangThai { get; set; }
        public string TenHangHoa { get; set; }
        public double SoLuong { get; set; }
        public double? DonGia { get; set; }
        public string? Hinh { get; set; }


    }
}
