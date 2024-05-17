using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KLTN_E.Data;

public partial class TrangThai
{
    public int MaTrangThai { get; set; }
    [Display(Name = "Status")]
    public string TenTrangThai { get; set; } = null!;
    [Display(Name = "Description")]
    public string? MoTa { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
