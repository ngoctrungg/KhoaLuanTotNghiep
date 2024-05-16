using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KLTN_E.Data;
using Microsoft.Extensions.Hosting;
using KLTN_E.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace KLTN_E.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HangHoasController : Controller
    {
        private readonly KltnContext _context;
        private readonly IWebHostEnvironment _environment;
        public HangHoasController(KltnContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [Authorize(Roles = "Admin, Employee")]
        // GET: Admin/HangHoas1
        public async Task<IActionResult> Index()
        {
            var kltnContext = _context.HangHoas.Include(h => h.MaLoaiNavigation).Include(h => h.MaNccNavigation);
            return View(await kltnContext.ToListAsync());
        }
        [Authorize(Roles = "Admin, Employee")]
        // GET: Admin/HangHoas1/Details/5
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
        [Authorize(Roles = "Admin")]
        // GET: Admin/HangHoas1/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Loai = new SelectList(_context.Loais, "MaLoai", "TenLoai");
            ViewBag.NCC = new SelectList(_context.NhaCungCaps, "MaNcc", "TenCongTy");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HangHoa hangHoa, IFormFile hinhHH)
        {

            ViewBag.Loai = new SelectList(_context.Loais, "MaLoai", "TenLoai", hangHoa.MaLoai);
            ViewBag.NCC = new SelectList(_context.NhaCungCaps, "MaNcc", "TenCongTy", hangHoa.MaNcc);
            var check = await _context.HangHoas.FirstOrDefaultAsync(p => p.TenHh == hangHoa.TenHh);
            if (check != null)
            {
                ModelState.AddModelError("TenHH", "San pham da ton tai");
                return View(hangHoa);
            }
            else
            {

                if (hinhHH != null)
                {
                    string dir = Path.Combine(_environment.WebRootPath, "Hinh/HangHoa");
                    string imgName = Guid.NewGuid().ToString() + hinhHH.FileName;
                    string filePath = Path.Combine(dir, imgName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await hinhHH.CopyToAsync(fs);
                    fs.Close();
                    hangHoa.Hinh = imgName;
                }
            }
            _context.Add(hangHoa);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        [Authorize(Roles = "Admin")]
        // GET: Admin/HangHoas1/Edit/5
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHh,TenHh,TenAlias,MaLoai,MoTaDonVi,DonGia,Hinh,NgaySx,GiamGia,SoLanXem,MoTa,MaNcc")] HangHoa hangHoa, IFormFile hinhHH)
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
               
                if (hinhHH != null)
                {
                    string dir = Path.Combine(_environment.WebRootPath, "Hinh/HangHoa");
                    string imgName = Guid.NewGuid().ToString() + hinhHH.FileName;
                    string filePath = Path.Combine(dir, imgName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await hinhHH.CopyToAsync(fs);
                    fs.Close();
                    existedHH.Hinh = imgName;
                }
                else
                {
                    hangHoa.Hinh = existedHH.Hinh;
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

        [Authorize(Roles = "Admin")]
        // GET: Admin/HangHoas1/Delete/5
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

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hangHoa = await _context.HangHoas.FindAsync(id);
            if (hangHoa != null)
            {
                if (!string.Equals(hangHoa.Hinh, "noImg.jpg"))
                {
                    string dir = Path.Combine(_environment.WebRootPath, "Hinh/HangHoa");
                    string oldfileImg = Path.Combine(dir, hangHoa.Hinh);
                    if (System.IO.File.Exists(oldfileImg))
                    {
                        System.IO.File.Delete(oldfileImg);
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
