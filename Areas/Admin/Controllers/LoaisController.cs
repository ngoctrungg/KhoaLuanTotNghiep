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
    public class LoaisController : Controller
    {
        private readonly KltnContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public LoaisController(KltnContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Loais
        public async Task<IActionResult> Index()
        {
            return View(await _context.Loais.ToListAsync());
        }

        // GET: Admin/Loais/Details/5
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

        // GET: Admin/Loais/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Loais/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Loai loai, IFormFile hinhL)
        {
            if (hinhL != null)
            {
                loai.Hinh = MyUtil.UploadHinh(hinhL, "Loai");

            }
            _context.Add(loai);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        // GET: Admin/Loais/Edit/5
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

        // POST: Admin/Loais/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Loai loai, IFormFile hinhL)
        {

            var existedLoai = _context.Loais.SingleOrDefault(x => x.MaLoai == loai.MaLoai);
            if (existedLoai != null)
            {
                //Edit
                existedLoai.TenLoai = loai.TenLoai;
                existedLoai.TenLoaiAlias = loai.TenLoaiAlias;
                existedLoai.MoTa = loai.MoTa;
                if (hinhL == null)
                {
                    existedLoai.Hinh = loai.Hinh;
                }
                else
                {
                    existedLoai.Hinh = MyUtil.UploadHinh(hinhL, "Loai");
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

        // GET: Admin/Loais/Delete/5
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

        // POST: Admin/Loais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loai = await _context.Loais.FindAsync(id);
            if (loai != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Hinh/Loai");
                string oldIMG = Path.Combine(uploadDir, loai.Hinh);
                if (System.IO.File.Exists(oldIMG))
                {
                    System.IO.File.Delete(oldIMG);
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
