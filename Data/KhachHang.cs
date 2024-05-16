using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KLTN_E.Data;

public partial class KhachHang
{
    [Display(Name = "User Name")]

    public string MaKh { get; set; } = null!;
    [Display(Name = "Password")]

    public string? MatKhau { get; set; }
    [Display(Name = "Full Name")]

    public string HoTen { get; set; } = null!;

    [Display(Name = "Boy?")]
    public bool GioiTinh { get; set; }
    [Display(Name = "Date Of Birth")]

    public DateTime NgaySinh { get; set; }
    [Display(Name = "Address")]

    public string? DiaChi { get; set; }
    [Display(Name = "Phone Number")]

    public string? DienThoai { get; set; }
    [Display(Name = "Email")]

    public string Email { get; set; } = null!;
    [Display(Name = "Image")]

    public string? Hinh { get; set; }

    [Display(Name = "Lockout Disable")]
    public bool HieuLuc { get; set; }
    [Display(Name = "Role")]

    public int VaiTro { get; set; }
    [Display(Name = "Random Key")]

    public string? RandomKey { get; set; }
    [Display(Name = "Reset Token")]

    public string? ResetToken { get; set; }

    public virtual ICollection<BanBe> BanBes { get; set; } = new List<BanBe>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual Role VaiTroNavigation { get; set; } = null!;

    public virtual ICollection<YeuThich> YeuThiches { get; set; } = new List<YeuThich>();
}
