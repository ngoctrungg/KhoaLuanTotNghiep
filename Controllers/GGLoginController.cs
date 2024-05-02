using KLTN_E.Data;
using KLTN_E.Helpers;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KLTN_E.Controllers
{
    public class GGLoginController : Controller
    {
        private readonly KltnContext db;

        public GGLoginController(KltnContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task GGLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Succeeded == true)
            {
                // Lấy thông tin từ Google
                var googleClaims = result.Principal.Identities.FirstOrDefault().Claims;

                // Xử lý thông tin người dùng
                var email = googleClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var fullName = googleClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var profileImage = googleClaims.FirstOrDefault(c => c.Type == "picture")?.Value;

                // Kiểm tra xem người dùng đã tồn tại trong hệ thống hay chưa
                var existingUser = db.KhachHangs.FirstOrDefault(u => u.Email == email);
                if (existingUser == null)
                {
                    // Tạo người dùng mới nếu chưa tồn tại
                    var newUser = new RegisterVM
                    {
                        Email = email,
                        HoTen = fullName,
                        Hinh = profileImage
                        // Bạn cũng có thể lưu hình ảnh hồ sơ của người dùng ở đây
                        // Hãy thêm một trường vào bảng KhachHang để lưu đường dẫn hình ảnh
                    };
                    db.Add(newUser);
                    db.SaveChanges();

                    // Tiến hành đăng nhập người dùng mới
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, newUser.Email),
                new Claim(ClaimTypes.Name, newUser.HoTen),
                new Claim(MySettings.CLAIM_CUSTOMER_ID, newUser.MaKh),
                // Thêm các claim khác tùy vào yêu cầu của ứng dụng của bạn
            };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                }
                else
                {
                    // Người dùng đã tồn tại, tiến hành đăng nhập
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, existingUser.Email),
                new Claim(ClaimTypes.Name, existingUser.HoTen),
                new Claim(MySettings.CLAIM_CUSTOMER_ID, existingUser.MaKh),
                // Thêm các claim khác tùy vào yêu cầu của ứng dụng của bạn
            };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                }

                // Chuyển hướng đến trang chính
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            // Xử lý trường hợp không thành công
            // Nếu cần, bạn có thể xử lý lỗi ở đây

            return RedirectToAction("Login");
        }


        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
        //    {
        //        claim.Issuer,
        //        claim.OriginalIssuer,
        //        claim.Type,
        //        claim.Value
        //    });

        //    return RedirectToAction("Index", "Home", new {area = ""});
        //}



    }
}
