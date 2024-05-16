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
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Index()
        {
            var Kltn = db.HoaDons.Include(p => p.MaTrangThaiNavigation);
            return View(await Kltn.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelledOrders()
        {
            var cancelOrder = await db.HoaDons.Include(p => p.MaTrangThaiNavigation).Where(p => p.MaTrangThai == -1).ToListAsync();

            return View(cancelOrder);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingOrders()
        {
            var pendingOrder = await db.HoaDons.Include(p => p.MaTrangThaiNavigation).Where(p => p.MaTrangThai == 0).ToListAsync();

            return View(pendingOrder);
        }
        
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApprovedOrders()
        {
            var approveOrder = await db.HoaDons.Include(p => p.MaTrangThaiNavigation).Where(p => p.MaTrangThai == 1).ToListAsync();

            return View(approveOrder);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingDelivery()
        {
            var pendingDelivery = await db.HoaDons.Include(p => p.MaTrangThaiNavigation).Where(p => p.MaTrangThai == 1).ToListAsync();

            return View(pendingDelivery);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delivered()
        {
            var Delivered = await db.HoaDons.Include(p => p.MaTrangThaiNavigation).Where(p => p.MaTrangThai == 2).ToListAsync();

            return View(Delivered);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewOrderDetail(int maHd)
        {
            var ct = await db.ChiTietHds.Include(p => p.MaHhNavigation).Where(ct => ct.MaHd == maHd).ToListAsync();
            return View(ct);
        }


    }
}
