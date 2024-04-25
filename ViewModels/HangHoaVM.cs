using KLTN_E.Data;

namespace KLTN_E.ViewModels
{
    public class HangHoaVM
    {
        public int MaHH { get; set; }
        public string TenHH { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public string TenLoai { get; set; }
    }
    public class ChiTietHangHoaVM
    {
        public int MaHH { get; set; }
        public string TenHH { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public string TenLoai { get; set; }
        public string MoTa { get; set; }
        public int DiemDanhGia { get; set; }
        public int SoLuongTon { get; set; }
        public string ChiTiet { get; set; }
        public List<Comment>? Comments { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
