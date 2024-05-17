using System;
using System.Collections.Generic;

namespace KLTN_E.Data;

public partial class Role
{
    public int MaVaiTro { get; set; }

    public string TenVaiTro { get; set; } = null!;

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
}
