using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KLTN_E.Data;

public partial class HoaDon
{
    [Display(Name = "OrderId")]

    public int MaHd { get; set; }
    [Display(Name = "CustomerId")]

    public string MaKh { get; set; } = null!;
    [Display(Name = "Order Date")]

    public DateTime NgayDat { get; set; }
    [Display(Name = "Delivery Date")]
    public DateTime? NgayCan { get; set; }
    [Display(Name = "Shipping Date")]
    public DateTime? NgayGiao { get; set; }
    [Display(Name = "Full Name")]

    public string? HoTen { get; set; }
    [Display(Name = "Address")]

    public string DiaChi { get; set; } = null!;
    [Display(Name = "Phone Number")]

    public string? DienThoai { get; set; }
    [Display(Name = "Payment Method")]

    public string CachThanhToan { get; set; } = null!;

    public string CachVanChuyen { get; set; } = null!;

    public double PhiVanChuyen { get; set; }
    [Display(Name = "Status")]
    public int MaTrangThai { get; set; }
    [Display(Name = "EmployeeId")]

    public string? MaNv { get; set; }
    [Display(Name = "Note")]

    public string? GhiChu { get; set; }

    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual NhanVien? MaNvNavigation { get; set; }

    public virtual TrangThai MaTrangThaiNavigation { get; set; } = null!;
}
