using KLTN_E.Data;
using KLTN_E.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KLTN_E.Services
{
    public class PurchaseHistoryService : IPurchaseHistoryService
    {
        private readonly KltnContext db;

        public PurchaseHistoryService(KltnContext context)
        {
            db = context;
        }
        public List<PurchaseHistoryVM> GetPurchaseHistory(string maKhachHang)
        {
            return db.HoaDons
                .Include(p => p.ChiTietHds)
                .Where(hd => hd.MaKh == maKhachHang)
                .SelectMany(p => p.ChiTietHds)
                .Select(chiTiet => new PurchaseHistoryVM
                {
                    NgayDat = chiTiet.MaHdNavigation.NgayDat,
                    TenTrangThai = chiTiet.MaHdNavigation.MaTrangThaiNavigation.TenTrangThai,
                    TenHangHoa = chiTiet.MaHhNavigation.TenHh,
                    DonGia = chiTiet.MaHhNavigation.DonGia,
                    Hinh = chiTiet.MaHhNavigation.Hinh,
                    SoLuong = chiTiet.SoLuong
                })
                .ToList();
        }
    }
}
