using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KLTN_E.Data;

public partial class NhaCungCap
{
    [Display(Name = "SupplierId")]

    public string MaNcc { get; set; } = null!;
    [Display(Name = "Supplier Name")]

    public string TenCongTy { get; set; } = null!;

    public string? Logo { get; set; } = "noImg.jpg";    
    [Display(Name = "Contact Person")]

    public string? NguoiLienLac { get; set; }

    public string Email { get; set; } = null!;
    [Display(Name = "Phone Number")]

    public string? DienThoai { get; set; }
    [Display(Name = "Address")]

    public string? DiaChi { get; set; }
    [Display(Name = "Description")]

    public string? MoTa { get; set; }

    public virtual ICollection<HangHoa> HangHoas { get; set; } = new List<HangHoa>();
}
