using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KLTN_E.Data;
using KLTN_E.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace KLTN_E.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HangHoasController : Controller
    {
        private readonly KltnContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HangHoasController(KltnContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/HangHoas
        public async Task<IActionResult> Index()
        {
            var kltnContext = _context.HangHoas.Include(h => h.MaLoaiNavigation).Include(h => h.MaNccNavigation);
            return View(await kltnContext.ToListAsync());
        }

        // GET: Admin/HangHoas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangHoa = await _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .Include(h => h.MaNccNavigation)
                .FirstOrDefaultAsync(m => m.MaHh == id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            return View(hangHoa);
        }

        // GET: Admin/HangHoas/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Loai = new SelectList(_context.Loais, "MaLoai", "TenLoai");
            ViewBag.NCC = new SelectList(_context.NhaCungCaps, "MaNcc", "TenCongTy");
            return View();
        }

        // POST: Admin/HangHoas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HangHoa hangHoa, IFormFile hinhHH)
        {
            ViewBag.Loai = new SelectList(_context.Loais, "MaLoai", "TenLoai");
            ViewBag.NCC = new SelectList(_context.NhaCungCaps, "MaNcc", "TenCongTy");

            var existingProduct = _context.HangHoas.FirstOrDefault(p => p.TenHh == hangHoa.TenHh);
            if (existingProduct != null)
            {
                ModelState.AddModelError("TenHh", "Tên sản phẩm đã tồn tại.");
                return View(hangHoa);
            }
            if (hinhHH != null)
            {
                hangHoa.Hinh = MyUtil.UploadHinh(hinhHH, "HangHoa");
            }
            _context.Add(hangHoa);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Tạo mới thành công.";
            return RedirectToAction("Index");



        }

        // GET: Admin/HangHoas/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangHoa = await _context.HangHoas.FindAsync(id);
            if (hangHoa == null)
            {
                return NotFound();
            }
            ViewBag.Loai = new SelectList(_context.Loais, "MaLoai", "TenLoai", hangHoa.MaLoai);
            ViewBag.NCC = new SelectList(_context.NhaCungCaps, "MaNcc", "TenCongTy", hangHoa.MaNcc);
            return View(hangHoa);
        }

        // POST: Admin/HangHoas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(HangHoa hangHoa, IFormFile hinhHH)
        {

            var existedHH = _context.HangHoas.SingleOrDefault(x => x.MaHh == hangHoa.MaHh);
            if (existedHH != null)
            {
                //Edit
                existedHH.TenHh = hangHoa.TenHh;
                existedHH.TenAlias = hangHoa.TenAlias;
                existedHH.MoTaDonVi = hangHoa.MoTaDonVi;
                existedHH.DonGia = hangHoa.DonGia;
                existedHH.GiamGia = hangHoa.GiamGia;
                existedHH.MoTa = hangHoa.MoTa;
                existedHH.MaLoai = hangHoa.MaLoai;
                existedHH.MaNcc = hangHoa.MaNcc;

                if (hinhHH == null)
                {
                    existedHH.Hinh = hangHoa.Hinh;
                }
                else
                {

                    existedHH.Hinh = MyUtil.UploadHinh(hinhHH, "HangHoa");
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

        // GET: Admin/HangHoas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangHoa = await _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .Include(h => h.MaNccNavigation)
                .FirstOrDefaultAsync(m => m.MaHh == id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            return View(hangHoa);
        }

        // POST: Admin/HangHoas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hangHoa = await _context.HangHoas.FindAsync(id);
            if (hangHoa != null)
            {
                if (!string.Equals(hangHoa.Hinh, "noimg.jpg"))
                {

                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Hinh/HangHoa");
                    string oldIMG = Path.Combine(uploadDir, hangHoa.Hinh);


                    if (System.IO.File.Exists(oldIMG))
                    {
                        System.IO.File.Delete(oldIMG);
                    }
                }

                _context.HangHoas.Remove(hangHoa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HangHoaExists(int id)
        {
            return _context.HangHoas.Any(e => e.MaHh == id);
        }
    }
}
