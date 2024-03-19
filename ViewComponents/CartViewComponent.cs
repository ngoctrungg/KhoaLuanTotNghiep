using KLTN_E.Helpers;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KLTN_E.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(MySettings.CART_KEY) ?? new List<CartItem>();
            return View("CartPanel", new CartModel
            {
                Quantity = cart.Sum(p => p.SoLuong),
                Total = cart.Sum(p => p.ThanhTien)
            });
        }
    }
}
