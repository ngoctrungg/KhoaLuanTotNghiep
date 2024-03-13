using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KLTN_E.Data;
using KLTN_E.Helpers;
using KLTN_E.ViewModels;

namespace KLTN_E.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangsController : Controller
    {
        private readonly KltnContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public KhachHangsController(KltnContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/KhachHangs
        public async Task<IActionResult> Index()
        {
            return View(await _context.KhachHangs.ToListAsync());
        }

        // GET: Admin/KhachHangs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // GET: Admin/KhachHangs/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/KhachHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang khachHang, IFormFile hinhKH)
        {
            if (ModelState.IsValid)
            {
                if (hinhKH != null)
                {
                    khachHang.Hinh = MyUtil.UploadHinh(hinhKH, "KhachHang");
                }
                _context.Add(khachHang);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(khachHang);
        }

        // GET: Admin/KhachHangs/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return View(khachHang);
        }

        // POST: Admin/KhachHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(KhachHang khachHang, IFormFile hinhKH)
        {
            var existedKhachHangs = _context.KhachHangs.SingleOrDefault(x => x.MaKh == khachHang.MaKh);
            if (existedKhachHangs != null)
            {
                //Edit
                existedKhachHangs.MatKhau = khachHang.MatKhau;
                existedKhachHangs.Email = khachHang.Email;
                existedKhachHangs.HoTen = khachHang.HoTen;
                existedKhachHangs.GioiTinh = khachHang.GioiTinh;
                existedKhachHangs.DiaChi = khachHang.DiaChi;
                existedKhachHangs.DienThoai = khachHang.DienThoai;
                existedKhachHangs.HieuLuc = khachHang.HieuLuc;
                existedKhachHangs.VaiTro = khachHang.VaiTro;
                existedKhachHangs.RandomKey = khachHang.RandomKey;


                if (hinhKH == null)
                {
                    existedKhachHangs.Hinh = khachHang.Hinh;
                }
                else
                {
                    existedKhachHangs.Hinh = MyUtil.UploadHinh(hinhKH, "KhachHang");
                }
                //Save
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }


        }

        // GET: Admin/KhachHangs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // POST: Admin/KhachHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Hinh/KhachHang");
                string oldIMG = Path.Combine(uploadDir, khachHang.Hinh);
                if (System.IO.File.Exists(oldIMG))
                {
                    System.IO.File.Delete(oldIMG);
                }
                _context.KhachHangs.Remove(khachHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachHangExists(string id)
        {
            return _context.KhachHangs.Any(e => e.MaKh == id);
        }
    }
}
