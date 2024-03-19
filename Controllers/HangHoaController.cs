using KLTN_E.Data;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KLTN_E.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly KltnContext db;

        public HangHoaController(KltnContext context) 
        {
            db = context;
        }
        public IActionResult Index(int? loai)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if(loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHH = p.MaHh,
                TenHH = p.TenHh,
                Hinh = p.Hinh ?? "",
                DonGia = p.DonGia ?? 0,
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            return View(result);
        }

        public IActionResult Search(string? query)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHH = p.MaHh,
                TenHH = p.TenHh,
                Hinh = p.Hinh ?? "",
                DonGia = p.DonGia ?? 0,
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);

            if(data == null)
            {
                TempData["Message"] = $"Không tìm thấy sản phẩm có mã là {id}";
                return Redirect("/404");
            }
            var result = new ChiTietHangHoaVM
            {
                MaHH = data.MaHh,
                TenHH = data.TenHh,
                DonGia = data.DonGia ?? 0,
                MoTa = data.MoTaDonVi ?? "",
                DiemDanhGia = 5,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                Hinh = data.Hinh ?? "",
                SoLuongTon = 10,
                ChiTiet = data.MoTa
            };
            return View(result);
        }

        public IActionResult Filter(string minPrice)
        {
            if (!string.IsNullOrEmpty(minPrice))
            {
                // Tách giá trị minPrice thành khoảng giá
                var priceRange = minPrice.Split('-');

                if (priceRange.Length == 2 && int.TryParse(priceRange[0], out var min) && int.TryParse(priceRange[1], out var max))
                {
                    // Lọc sản phẩm có giá trong khoảng từ min đến max
                    
                    var filteredProducts = db.HangHoas
                        .Where(p => p.DonGia != null && p.DonGia.Value >= min && p.DonGia.Value <= max)
                        .Select(p => new HangHoaVM
                        {
                            MaHH = p.MaHh,
                            TenHH = p.TenHh,
                            Hinh = p.Hinh ?? "",
                            DonGia = p.DonGia ?? 0,
                            TenLoai = p.MaLoaiNavigation.TenLoai ?? "..."
                        })
                        .ToList();

                    // Trả về danh sách sản phẩm đã lọc đến View
                    return View(filteredProducts);
                }
            }

            // Nếu không có giá tối thiểu được chọn, trả về toàn bộ danh sách sản phẩm
            var allProducts = db.HangHoas
                .Select(p => new HangHoaVM
                {
                    MaHH = p.MaHh,
                    TenHH = p.TenHh,
                    Hinh = p.Hinh,
                    DonGia = p.DonGia ?? 0,
                    TenLoai = p.MaLoaiNavigation.TenLoai
                })
                .ToList();

            return View(allProducts);
        }



    }
}
