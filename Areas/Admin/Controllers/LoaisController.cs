using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KLTN_E.Data;
using Microsoft.AspNetCore.Authorization;

namespace KLTN_E.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoaisController : Controller
    {
        private readonly KltnContext _context;
        private readonly IWebHostEnvironment _environment;
        public LoaisController(KltnContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Loais.ToListAsync());
        }

        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loai = await _context.Loais
                .FirstOrDefaultAsync(m => m.MaLoai == id);
            if (loai == null)
            {
                return NotFound();
            }

            return View(loai);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLoai,TenLoai,TenLoaiAlias,MoTa,Hinh")] Loai loai, IFormFile hinhLoai)
        {
            //if (ModelState.IsValid)
            //   {
            var check = await _context.Loais.FirstOrDefaultAsync(p => p.TenLoai == loai.TenLoai);
            if (check != null)
            {
                ModelState.AddModelError("TenLoai", "San pham da ton tai");
                return View(loai);
            }
            else
            {

                if (hinhLoai != null)
                {
                    string dir = Path.Combine(_environment.WebRootPath, "Hinh/Loai");
                    string imgName = Guid.NewGuid().ToString() + hinhLoai.FileName;
                    string filePath = Path.Combine(dir, imgName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await hinhLoai.CopyToAsync(fs);
                    fs.Close();
                    loai.Hinh = imgName;
                }
            }
            _context.Add(loai);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //  }
            //  return View(loai);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loai = await _context.Loais.FindAsync(id);
            if (loai == null)
            {
                return NotFound();
            }
            return View(loai);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLoai,TenLoai,TenLoaiAlias,MoTa,Hinh")] Loai loai, IFormFile hinhLoai)
        {
            if (id != loai.MaLoai)
            {
                return NotFound();
            }
            var existedCategory = await _context.Loais.AsNoTracking().FirstOrDefaultAsync(kh => kh.MaLoai == loai.MaLoai);
            try
            {
                if (hinhLoai != null)
                {
                    string dir = Path.Combine(_environment.WebRootPath, "Hinh/Loai");
                    string imgName = Guid.NewGuid().ToString() + hinhLoai.FileName;
                    string filePath = Path.Combine(dir, imgName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await hinhLoai.CopyToAsync(fs);
                    fs.Close();
                    loai.Hinh = imgName;
                }
                else
                {
                    loai.Hinh = existedCategory.Hinh;
                }

                _context.Update(loai);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoaiExists(loai.MaLoai))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loai = await _context.Loais
                .FirstOrDefaultAsync(m => m.MaLoai == id);
            if (loai == null)
            {
                return NotFound();
            }

            return View(loai);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loai = await _context.Loais.FindAsync(id);
            if (loai != null)
            {
                if (!string.Equals(loai.Hinh, "noImg.jpg"))
                {
                    string dir = Path.Combine(_environment.WebRootPath, "Hinh/Loai");
                    string oldfileImg = Path.Combine(dir, loai.Hinh);
                    if (System.IO.File.Exists(oldfileImg))
                    {
                        System.IO.File.Delete(oldfileImg);
                    }
                }
                _context.Loais.Remove(loai);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaiExists(int id)
        {
            return _context.Loais.Any(e => e.MaLoai == id);
        }
    }
}
