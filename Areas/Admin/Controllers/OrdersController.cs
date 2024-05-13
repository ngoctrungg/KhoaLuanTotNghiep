using KLTN_E.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KLTN_E.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly KltnContext db;

        public OrdersController(KltnContext context)
        {
            db = context;
        }
        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> Index()
        {
            var Kltn = db.HoaDons.Include(p => p.MaTrangThaiNavigation);
            return View(await Kltn.ToListAsync());
        }

        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> ApproveOrder(int? maHd)
        {
            var order = await db.HoaDons.FindAsync(maHd);
            if (order == null)
            {
                return NotFound();
            }

            order.MaTrangThai = 1;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> CancelledOrders()
        {
            var cancelOrder = await db.HoaDons.Include(p => p.MaTrangThaiNavigation).Where(p => p.MaTrangThai == -1).ToListAsync();

            return View(cancelOrder);
        }

        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> PendingOrders()
        {
            var cancelOrder = await db.HoaDons.Include(p => p.MaTrangThaiNavigation).Where(p => p.MaTrangThai == 0).ToListAsync();

            return View(cancelOrder);
        }
        
        
        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> ApprovedOrders()
        {
            var cancelOrder = await db.HoaDons.Include(p => p.MaTrangThaiNavigation).Where(p => p.MaTrangThai == 1).ToListAsync();

            return View(cancelOrder);
        }


        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> PendingDelivery()
        {
            var cancelOrder = await db.HoaDons.Include(p => p.MaTrangThaiNavigation).Where(p => p.MaTrangThai == 1).ToListAsync();

            return View(cancelOrder);
        }

        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> Delivered()
        {
            var cancelOrder = await db.HoaDons.Include(p => p.MaTrangThaiNavigation).Where(p => p.MaTrangThai == 2).ToListAsync();

            return View(cancelOrder);
        }

        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> UpdateOrders(int maHd)
        {
            var order = await db.HoaDons.FindAsync(maHd);
            if (order == null)
            {
                return NotFound();
            }

            if (order.MaTrangThai == 1 && order.NgayGiao > order.NgayDat)
            {
                order.MaTrangThai = 2;
            }

            await db.SaveChangesAsync();

            return RedirectToAction("PendingDelivery");
        }



    }
}
