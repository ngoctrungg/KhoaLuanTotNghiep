using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KLTN_E.Data;

public partial class HangHoa
{
    public int MaHh { get; set; }
    [Display(Name = "Product")]

    public string TenHh { get; set; } = null!;
    [Display(Name = "Alias Name")]

    public string? TenAlias { get; set; }
    [Display(Name = "Category")]

    public int MaLoai { get; set; }
    [Display(Name = "Sort Description")]

    public string? MoTaDonVi { get; set; }
    [Display(Name = "Price")]

    public double? DonGia { get; set; }
    [Display(Name = "Image")]

    public string? Hinh { get; set; }
    [Display(Name = "Production Date")]

    public DateTime NgaySx { get; set; }
    [Display(Name = "Sale")]

    public double GiamGia { get; set; }
    [Display(Name = "Views")]

    public int SoLanXem { get; set; }
    [Display(Name = "Description")]

    public string? MoTa { get; set; }
    [Display(Name = "Supplier")]

    public string MaNcc { get; set; } = null!;


    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();

    public virtual Loai MaLoaiNavigation { get; set; } = null!;

    public virtual NhaCungCap MaNccNavigation { get; set; } = null!;

    public List<Comment>? Comments { get; set; }
}
