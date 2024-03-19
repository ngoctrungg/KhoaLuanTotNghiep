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
    public class NhanViensController : Controller
    {
        private readonly KltnContext _context;

        public NhanViensController(KltnContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.NhanViens.ToListAsync());
        }

        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
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
        public async Task<IActionResult> Create(NhanVien nhanVien)
        {

            _context.Add(nhanVien);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return View(nhanVien);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NhanVien nhanVien, string id)
        {
            var existedNV = _context.NhanViens.SingleOrDefault(x => x.MaNv == id);
            if (existedNV != null)
            {
                //Edit
                existedNV.HoTen = nhanVien.HoTen;
                existedNV.Email = nhanVien.Email;
                existedNV.MatKhau = nhanVien.MatKhau;

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
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien != null)
            {
                _context.NhanViens.Remove(nhanVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienExists(string id)
        {
            return _context.NhanViens.Any(e => e.MaNv == id);
        }
    }
}
