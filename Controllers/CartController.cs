using KLTN_E.Data;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Mvc;
using KLTN_E.Helpers;

namespace KLTN_E.Controllers
{
    public class CartController : Controller
    {
        private readonly KltnContext db;

        public CartController(KltnContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View(Cart);
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySettings.CART_KEY) ?? new List<CartItem>();
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaHh = hangHoa.MaHh,
                    TenHh = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? "",
                    SoLuong = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }
            HttpContext.Session.Set(MySettings.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                    gioHang.Remove(item);
            }
            HttpContext.Session.Set(MySettings.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }
    }
}
