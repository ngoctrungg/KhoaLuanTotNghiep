using AutoMapper;
using KLTN_E.Data;
using KLTN_E.Helpers;
using KLTN_E.Services;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace KLTN_E.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly KltnContext db;
        private readonly IMapper _mapper;
        private readonly IEmailSender _myEmailSender;

        public KhachHangController(KltnContext context, IMapper mapper, IEmailSender emailSender)
        {
            db = context;
            _mapper = mapper;
            _myEmailSender = emailSender;
        }
        #region Register
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangKy(RegisterVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtil.GenerateRandomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true;
                    khachHang.VaiTro = 0;

                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }
                    db.Add(khachHang);
                    db.SaveChanges();
                

                    var userId = khachHang.MaKh;
                    var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userId.ToString()));
                    var callBackUrl = Url.Action("ConfirmEmail", "KhachHang", new { UserId = userId, code = code }, protocol: HttpContext.Request.Scheme);
                    await _myEmailSender.SendEmailAsync(model.Email, "Confirm your Email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callBackUrl)}'>clicking here</a>.");
                    return RedirectToAction("ConfirmEmail");


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View();
        }
        #endregion

        [HttpGet]
        public IActionResult ConfirmEmail()
        {
            return View();
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("/404");
            }

            var user = await db.KhachHangs.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("/404");
            }
            if (!user.HieuLuc)
            {
                user.HieuLuc = true;
                await db.SaveChangesAsync();
                return RedirectToAction("EmailConfirmed");

            }
            return View();
        }



        #region Login
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
                if (khachHang == null)
                {
                    ModelState.AddModelError("Error", "UserName or Password is not correct");
                }
                else
                {
                    if (!khachHang.HieuLuc)
                    {
                        ModelState.AddModelError("Error", "Account has been locked");
                    }
                    else
                    {
                        if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
                        {
                            ModelState.AddModelError("Error", "UserName or Password is not correct");
                        }
                        else
                        {
                            var claims = new List<Claim> {
                                new Claim(ClaimTypes.Email, khachHang.Email),
                                new Claim(ClaimTypes.Name, khachHang.HoTen),
                                new Claim(MySettings.CLAIM_CUSTOMER_ID, khachHang.MaKh),

                                // Claim - role động
                                new Claim(ClaimTypes.Role, "Customer")
                            };
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(claimsPrincipal);
                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                    }
                }
            }
            return View();
        }
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        public IActionResult RSPasswordSuccess()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        #region QuenMatKhau
        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QuenMatKhau(QuenMatKhauVM model)
        {
            var khachHang = db.KhachHangs.SingleOrDefault(k => k.MaKh == model.MaKh);
            

            if (khachHang != null)
            {
                // tạo và lưu token đặt lại mật khẩu
                khachHang.ResetToken = MyUtil.GenerateRandomKey();
                db.SaveChanges();

                TempData["UserId"] = khachHang.MaKh;
                TempData["Token"] = khachHang.ResetToken;

                var callbackUrl = Url.Action("DatLaiMatKhau", "KhachHang", new { userId = khachHang.MaKh, token = khachHang.ResetToken }, protocol: HttpContext.Request.Scheme);
                //var callbackUrl = Url.Action("DatLaiMatKhau", "KhachHang", new { userId = khachHang.MaKh, token = khachHang.ResetToken }, HttpContext.Request.Scheme);

                await _myEmailSender.SendEmailAsync(khachHang.Email, "Đặt lại mật khẩu", $"Vui lòng đặt lại mật khẩu của bạn bằng cách <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


                return RedirectToAction("DatLaiMatKhau");
            }
            else
            {
                ModelState.AddModelError("Error", "MaKh does not exist.");
                return View();
            }
        }

        [HttpGet]
        public IActionResult DatLaiMatKhau()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DatLaiMatKhau( DatLaiMatKhauVM model)
        {
            var userId = TempData["UserId"]?.ToString();
            var token = TempData["Token"]?.ToString();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                ViewBag.ErrrorMessage = "Dữ liệu không hợp lệ, vui lòng thử lại";
                return View("Error");
            }

            //Kiểm tra tính hợp lệ của token và userId
            var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == userId && kh.ResetToken == token);

            if (khachHang != null)
            {
                string hashedOldPassword = model.OldPassword.ToMd5Hash(khachHang.RandomKey);
                if (hashedOldPassword != khachHang.MatKhau)
                {
                    ModelState.AddModelError("Error", "old password is not correct.");
                    return View();
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("Error", "New password and confirm password do not match.");
                    return View();
                }

                // đặt lại mật khẩu và xóa token
                //khachHang.MatKhau = model.NewPassword.ToMd5Hash(khachHang.RandomKey);

                khachHang.MatKhau = model.NewPassword.ToMd5Hash(khachHang.RandomKey);
                khachHang.ResetToken = null;
                db.SaveChanges();

                return RedirectToAction("RSPasswordSuccess");
            }
            else
            {
                ViewBag.ErrrorMessage = "UserName Not found.";
                return View("Error");
            }
        }
        #endregion
    }
}
