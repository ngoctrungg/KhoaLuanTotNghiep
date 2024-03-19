using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KLTN_E.Data;
using Microsoft.Extensions.Hosting;

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

        // GET: Admin/NhaCungCaps
        public async Task<IActionResult> Index()
        {
            return View(await _context.NhaCungCaps.ToListAsync());
        }

        // GET: Admin/NhaCungCaps/Details/5
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

        // GET: Admin/NhaCungCaps/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NhaCungCaps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhaCungCap nhaCungCap, IFormFile hinhNCC)
        {
            if (ModelState.IsValid)
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
            return View(nhaCungCap);
        }

        // GET: Admin/NhaCungCaps/Edit/5
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

        // POST: Admin/NhaCungCaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NhaCungCap nhaCungCap, IFormFile hinhNCC)
        {
            if (id != nhaCungCap.MaNcc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
            return View(nhaCungCap);
        }

        // GET: Admin/NhaCungCaps/Delete/5
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

        // POST: Admin/NhaCungCaps/Delete/5
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
