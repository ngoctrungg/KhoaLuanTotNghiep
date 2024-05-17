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
        //public List<PurchaseHistoryVM> GetPurchaseHistory(string maKhachHang)
        //{
        //    return db.HoaDons
        //        .Include(p => p.ChiTietHds)
        //        .Where(hd => hd.MaKh == maKhachHang)
        //        .SelectMany(p => p.ChiTietHds)
        //        .Select(chiTiet => new PurchaseHistoryVM
        //        {
        //            NgayDat = chiTiet.MaHdNavigation.NgayDat,
        //            TenTrangThai = chiTiet.MaHdNavigation.MaTrangThaiNavigation.TenTrangThai,
        //            TenHangHoa = chiTiet.MaHhNavigation.TenHh,
        //            DonGia = chiTiet.MaHhNavigation.DonGia,
        //            Hinh = chiTiet.MaHhNavigation.Hinh,
        //            SoLuong = chiTiet.SoLuong
        //        })
        //        .ToList();
        //}

        //public List<PurchaseHistoryVM> GetPurchaseHistory(string maKhachHang)
        //{
        //    var purchaseHistoryList = new List<PurchaseHistoryVM>();

        //    var hoaDons = db.HoaDons
        //        .Include(hd => hd.ChiTietHds)
        //        .Where(hd => hd.MaKh == maKhachHang)
        //        .ToList();

        //    foreach (var hoaDon in hoaDons)
        //    {
        //        var purchaseHistoryItem = new PurchaseHistoryVM
        //        {
        //            MaHd = hoaDon.MaHd,
        //            NgayDat = hoaDon.NgayDat,
        //            TenTrangThai = hoaDon.MaTrangThaiNavigation.TenTrangThai,
        //            SanPhams = hoaDon.ChiTietHds.Select(chiTiet => new ProductViewModel
        //            {
        //                TenHangHoa = chiTiet.MaHhNavigation.TenHh,
        //                DonGia = chiTiet.MaHhNavigation.DonGia,
        //                Hinh = chiTiet.MaHhNavigation.Hinh,
        //                SoLuong = chiTiet.SoLuong
        //            }).ToList()
        //        };

        //        purchaseHistoryList.Add(purchaseHistoryItem);
        //    }

        //    return purchaseHistoryList;
        //}
        public List<PurchaseHistoryVM> GetPurchaseHistory(string maKhachHang)
        {
            var purchaseHistoryList = new List<PurchaseHistoryVM>();

            var hoaDons = db.HoaDons
                .Include(hd => hd.ChiTietHds)
                .Where(hd => hd.MaKh == maKhachHang)
                .ToList();

            foreach (var hoaDon in hoaDons)
            {
                var purchaseHistoryItem = new PurchaseHistoryVM
                {
                    MaHd = hoaDon.MaHd,
                    NgayDat = hoaDon.NgayDat,
                    //TenTrangThai = hoaDon.MaTrangThaiNavigation != null ? hoaDon.MaTrangThaiNavigation.TenTrangThai : "Unknown",
                    TenTrangThai = db.TrangThais.FirstOrDefault(tt => tt.MaTrangThai == hoaDon.MaTrangThai)?.TenTrangThai ?? "Unknown",
                    SanPhams = hoaDon.ChiTietHds
                    .Where(chiTiet => chiTiet.MaHh != null)
                    .Select(chiTiet => new ProductViewModel
                    {
                        TenHangHoa = db.HangHoas.FirstOrDefault(hh => hh.MaHh == chiTiet.MaHh)?.TenHh ?? "Unknown",
                        DonGia = db.HangHoas.FirstOrDefault(hh => hh.MaHh == chiTiet.MaHh)?.DonGia ?? 0,
                        Hinh = db.HangHoas.FirstOrDefault(hh => hh.MaHh == chiTiet.MaHh)?.Hinh ?? "",
                        SoLuong = chiTiet.SoLuong
                    })
                    .ToList()
                };

                purchaseHistoryList.Add(purchaseHistoryItem);
            }

            return purchaseHistoryList;
        }

    }
}
