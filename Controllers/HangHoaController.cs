using KLTN_E.Data;
using KLTN_E.Models;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace KLTN_E.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly KltnContext db;

        public HangHoaController(KltnContext context)
        {
            db = context;
        }
        public IActionResult Index(int? loai, int page = 1, int pageSize = 10)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }

            var totalHangHoas = hangHoas.Count();
            var totalPages = (int)Math.Ceiling((double)totalHangHoas / pageSize);

            if (page < 1)
            {
                page = 1;
            }

            if (page > totalPages)
            {
                page = totalPages;
            }

            var hangHoasPage = hangHoas
                .OrderBy(p => p.TenHh)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new HangHoaVM
                {
                    MaHH = p.MaHh,
                    TenHH = p.TenHh,
                    Hinh = p.Hinh ?? "",
                    DonGia = p.DonGia ?? 0,
                    TenLoai = p.MaLoaiNavigation.TenLoai
                })
                .ToList();

            var result = new PagedList<HangHoaVM>(hangHoasPage, totalHangHoas, page, pageSize, loai);
            return View(result);
        }

        //public IActionResult Search(string? query)
        //{
        //    var hangHoas = db.HangHoas.AsQueryable();

        //    if (query != null)
        //    {
        //        hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
        //    }
        //    var result = hangHoas.Select(p => new HangHoaVM
        //    {
        //        MaHH = p.MaHh,
        //        TenHH = p.TenHh,
        //        Hinh = p.Hinh ?? "",
        //        DonGia = p.DonGia ?? 0,
        //        TenLoai = p.MaLoaiNavigation.TenLoai
        //    });
        //    return View(result);
        //}
        public IActionResult Search(string? query, int page = 1, int pageSize = 10)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }
            ViewBag.Query = query;
            var totalHangHoas = hangHoas.Count();
            var totalPages = (int)Math.Ceiling((double)totalHangHoas / pageSize);

            if (page < 1)
            {
                page = 1;
            }

            if (page > totalPages)
            {
                page = totalPages;
            }

            var hangHoasPage = hangHoas
                .OrderBy(p => p.TenHh)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new HangHoaVM
                {
                    MaHH = p.MaHh,
                    TenHH = p.TenHh,
                    Hinh = p.Hinh ?? "",
                    DonGia = p.DonGia ?? 0,
                    TenLoai = p.MaLoaiNavigation.TenLoai
                })
                .ToList();

            var pagedList = new PagedList<HangHoaVM>(hangHoasPage, totalHangHoas, page, pageSize, null); // Tạo đối tượng PagedList

            return View(pagedList);
        }

        public IActionResult Detail(int id, int page = 1, int pageSize = 5)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);

            if (data == null)
            {
                TempData["Message"] = $"Không tìm thấy sản phẩm có mã là {id}";
                return Redirect("/404");
            }
            var commentQuery = db.Comments.Where(p => p.ProductId == id);
            var totalComments = commentQuery.Count();
            var totalPages = (int)Math.Ceiling((double)totalComments / pageSize);

            if (page < 1)
            {
                page = 1;
            }

            // Đảm bảo rằng không vượt quá số trang tổng cộng
            if (page > totalPages)
            {
                page = totalPages;
            }

            if(pageSize < 1)
            {
                pageSize = 1;
            }

            if(totalComments > 0)
            {
                var comments = commentQuery
              .Where(p => p.ProductId == id)
              .OrderByDescending(c => c.CreatedDate)
              .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToList();
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
                    ChiTiet = data.MoTa,
                    Comments = comments,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize
                };
                return View(result);
            } 
           else
            {
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
                    ChiTiet = data.MoTa,
                    Comments = new List<Comment>(),
                    TotalPages = 0,
                    CurrentPage = 1,
                    PageSize = pageSize
                };
                return View(result);
            }
        }

        //public IActionResult Filter(string minPrice)
        //{
        //    if (!string.IsNullOrEmpty(minPrice))
        //    {
        //        // Tách giá trị minPrice thành khoảng giá
        //        var priceRange = minPrice.Split('-');

        //        if (priceRange.Length == 2 && int.TryParse(priceRange[0], out var min) && int.TryParse(priceRange[1], out var max))
        //        {
        //            // Lọc sản phẩm có giá trong khoảng từ min đến max

        //            var filteredProducts = db.HangHoas
        //                .Where(p => p.DonGia != null && p.DonGia.Value >= min && p.DonGia.Value <= max)
        //                .Select(p => new HangHoaVM
        //                {
        //                    MaHH = p.MaHh,
        //                    TenHH = p.TenHh,
        //                    Hinh = p.Hinh ?? "",
        //                    DonGia = p.DonGia ?? 0,
        //                    TenLoai = p.MaLoaiNavigation.TenLoai ?? "..."
        //                })
        //                .ToList();

        //            // Trả về danh sách sản phẩm đã lọc đến View
        //            return View(filteredProducts);
        //        }
        //    }

        //    // Nếu không có giá tối thiểu được chọn, trả về toàn bộ danh sách sản phẩm
        //    var allProducts = db.HangHoas
        //        .Select(p => new HangHoaVM
        //        {
        //            MaHH = p.MaHh,
        //            TenHH = p.TenHh,
        //            Hinh = p.Hinh,
        //            DonGia = p.DonGia ?? 0,
        //            TenLoai = p.MaLoaiNavigation.TenLoai
        //        })
        //        .ToList();

        //    return View(allProducts);
        //}

        public IActionResult Filter(string minPrice, int? loai, int page = 1, int pageSize = 10)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            // Lọc theo danh mục nếu có giá trị của loai
            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }

            // Lọc theo giá nếu có giá trị minPrice được truyền vào
            if (!string.IsNullOrEmpty(minPrice))
            {
                var priceRange = minPrice.Split('-');

                if (priceRange.Length == 2 && int.TryParse(priceRange[0], out var min) && int.TryParse(priceRange[1], out var max))
                {
                    hangHoas = hangHoas.Where(p => p.DonGia != null && p.DonGia >= min && p.DonGia <= max);
                }
            }
            ViewBag.MinPrice = minPrice;
       
            var totalHangHoas = hangHoas.Count();
            var totalPages = (int)Math.Ceiling((double)totalHangHoas / pageSize);

            if (page < 1)
            {
                page = 1;
            }

            if (page > totalPages)
            {
                page = totalPages;
            }

            var hangHoasPage = hangHoas
                .OrderBy(p => p.TenHh)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new HangHoaVM
                {
                    MaHH = p.MaHh,
                    TenHH = p.TenHh,
                    Hinh = p.Hinh ?? "",
                    DonGia = p.DonGia ?? 0,
                    TenLoai = p.MaLoaiNavigation.TenLoai
                })
                .ToList();

            var pagedList = new PagedList<HangHoaVM>(hangHoasPage, totalHangHoas, page, pageSize, loai); // Tạo đối tượng PagedList

            return View(pagedList);
        }
    }
}
