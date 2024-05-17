using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KLTN_E.Data;

public partial class Loai
{
    public int MaLoai { get; set; }
    [Display(Name = "Category Name")]

    public string TenLoai { get; set; } = null!;
    [Display(Name = "Alias Name")]

    public string? TenLoaiAlias { get; set; }
    [Display(Name = "Description")]

    public string? MoTa { get; set; }
    [Display(Name = "Image  ")]

    public string? Hinh { get; set; }

    public virtual ICollection<HangHoa> HangHoas { get; set; } = new List<HangHoa>();
}
