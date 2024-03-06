using KLTN_E.Data;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Mvc;
using KLTN_E.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace KLTN_E.Controllers
{
    public class CartController : Controller
    {
        private readonly PaypalClient _paypalClient;
        private readonly KltnContext db;

        public CartController(KltnContext context, PaypalClient paypalClient)
        {
            _paypalClient = paypalClient;
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

        #region Checkout
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }
            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerID = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySettings.CLAIM_CUSTOMER_ID).Value;
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(p => p.MaKh == customerID);
                }
                var hoadon = new HoaDon
                {
                    MaKh = customerID,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    DienThoai = model.DienThoai ?? khachHang.DienThoai,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "GiaoHangNhanh",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu
                };
                    db.Database.BeginTransaction();
                try
                {
                    db.Database.CommitTransaction();
                    db.Add(hoadon);
                    db.SaveChanges();

                    var cthd = new List<ChiTietHd>();
                    foreach(var item in Cart)
                    {
                        cthd.Add(new ChiTietHd
                        {
                            MaHd = hoadon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                            GiamGia = 0
                        });
                    }
                    db.AddRange(cthd);
                    db.SaveChanges();
                    HttpContext.Session.Set<List<CartItem>>(MySettings.CART_KEY, new List<CartItem>());

                    return View("Success"); 
                }
                catch (Exception ex)
                {
                    db.Database.RollbackTransaction();
                }
            }
            return View(Cart);
        }
        #endregion


        #region Paypal
        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }

        [Authorize]
        [HttpPost("/Cart/create-paypal-order")]

        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            // thông tin đơn hàng gửi qua paypal
            var tongTien = Cart.Sum(p => p.ThanhTien).ToString();
            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);

                // lưu vào database
                

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        #endregion
    }
}
