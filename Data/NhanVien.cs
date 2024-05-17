using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KLTN_E.Data;

public partial class NhanVien
{
    [Display(Name = "EmployeeId")]
    public string MaNv { get; set; } = null!;
    [Display(Name = "Full Name")]
    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;
    [Display(Name = "Password")]
    public string? MatKhau { get; set; }


    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

}
