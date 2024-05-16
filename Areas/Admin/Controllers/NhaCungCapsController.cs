using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KLTN_E.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace KLTN_E.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhaCungCapsController : Controller
    {
        private readonly KltnContext _context;
        private readonly IWebHostEnvironment _environment;
        public NhaCungCapsController(KltnContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.NhaCungCaps.ToListAsync());
        }

        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCaps
                .FirstOrDefaultAsync(m => m.MaNcc == id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }

            return View(nhaCungCap);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhaCungCap nhaCungCap, IFormFile hinhNCC)
        {

            var check = await _context.NhaCungCaps.FirstOrDefaultAsync(p => p.TenCongTy == nhaCungCap.TenCongTy);
            if (check != null)
            {
                ModelState.AddModelError("TenCongTy", "Ten Cong Ty da ton tai");
                return View(nhaCungCap);
            }
            else
            {

                if (hinhNCC != null)
                {
                    string dir = Path.Combine(_environment.WebRootPath, "Hinh/NhaCC");
                    string imgName = Guid.NewGuid().ToString() + hinhNCC.FileName;
                    string filePath = Path.Combine(dir, imgName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await hinhNCC.CopyToAsync(fs);
                    fs.Close();
                    nhaCungCap.Logo = imgName;
                }
            }
            _context.Add(nhaCungCap);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }
            return View(nhaCungCap);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NhaCungCap nhaCungCap, IFormFile hinhNCC)
        {
            if (id != nhaCungCap.MaNcc)
            {
                return NotFound();
            }
            var existedSupplier = await _context.NhaCungCaps.AsNoTracking().FirstOrDefaultAsync(kh => kh.MaNcc == nhaCungCap.MaNcc);
            try
            {
                if (hinhNCC != null)
                {
                    string dir = Path.Combine(_environment.WebRootPath, "Hinh/NhaCC");
                    string imgName = Guid.NewGuid().ToString() + hinhNCC.FileName;
                    string filePath = Path.Combine(dir, imgName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await hinhNCC.CopyToAsync(fs);
                    fs.Close();
                    nhaCungCap.Logo = imgName;
                }
                else
                {
                    nhaCungCap.Logo = existedSupplier.Logo;
                }

                _context.Update(nhaCungCap);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhaCungCapExists(nhaCungCap.MaNcc))
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
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCaps
                .FirstOrDefaultAsync(m => m.MaNcc == id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }

            return View(nhaCungCap);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
            if (nhaCungCap != null)
            {
                if (!string.Equals(nhaCungCap.Logo, "noImg.jpg"))
                {
                    string dir = Path.Combine(_environment.WebRootPath, "Hinh/NhaCC");
                    string oldfileImg = Path.Combine(dir, nhaCungCap.Logo);
                    if (System.IO.File.Exists(oldfileImg))
                    {
                        System.IO.File.Delete(oldfileImg);
                    }
                }
                _context.NhaCungCaps.Remove(nhaCungCap);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhaCungCapExists(string id)
        {
            return _context.NhaCungCaps.Any(e => e.MaNcc == id);
        }
    }
}
