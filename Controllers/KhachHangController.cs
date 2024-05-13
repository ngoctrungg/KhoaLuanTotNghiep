using AutoMapper;
using KLTN_E.Data;
using KLTN_E.Helpers;
using KLTN_E.Models;
using KLTN_E.Services;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using PagedList;
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
        private readonly IPurchaseHistoryService _purchaseHistory;

        public KhachHangController(KltnContext context, IMapper mapper, IEmailSender emailSender, IPurchaseHistoryService purchaseHistory)
        {
            db = context;
            _mapper = mapper;
            _myEmailSender = emailSender;
            _purchaseHistory = purchaseHistory;
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
                    khachHang.HieuLuc = false;
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
                    return RedirectToAction("Notification");


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError(string.Empty, "An error occurred while registering. Please try again later.");
                    return View(model);
                }
            }
            return View();
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Notification()
        {
            return View();
        }



        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var user = await db.KhachHangs.FindAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            user.HieuLuc = true;
            db.Update(user);
            await db.SaveChangesAsync();

            return RedirectToAction("DangNhap", "KhachHang");
        }

        #region Login
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl, int vaiTro = 0)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
                var role = db.Roles.SingleOrDefault(r => r.MaVaiTro == khachHang.VaiTro);
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
                               // new Claim(ClaimTypes.Role, "Customer")
                            };

                            switch (role.MaVaiTro)
                            {
                                case 1:
                                    claims.Add(new Claim(ClaimTypes.Role, role.TenVaiTro));
                                    break;
                                case 2:
                                    claims.Add(new Claim(ClaimTypes.Role, role.TenVaiTro));
                                    break;
                                default:
                                    claims.Add(new Claim(ClaimTypes.Role, role.TenVaiTro));
                                    break;
                            }




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
        public IActionResult Profile(int page = 1, int pageSize = 5)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == MySettings.CLAIM_CUSTOMER_ID);
            if (userIdClaim != null)
            {
                var userId = userIdClaim.Value;
                var khachHang = db.KhachHangs.Find(userId);

                if (khachHang != null)
                {
                    var profileModel = new DatLaiMatKhauVM
                    {
                        UserName = khachHang.MaKh,
                        Email = khachHang.Email,
                        FullName = khachHang.HoTen,
                        Address = khachHang.DiaChi ?? "",
                        PhoneNumber = khachHang.DienThoai ?? "",
                        ProfileImage = khachHang.Hinh ?? ""
                    };

                    // Truy vấn dữ liệu mua hàng theo trang và kích thước trang
                    var purchaseHistoryQuery = _purchaseHistory.GetPurchaseHistory(userId)
                        .OrderByDescending(ph => ph.NgayDat); // Sắp xếp theo ngày mua giảm dần

                    // Tính toán tổng số mục mua hàng và tổng số trang
                    var totalPurchaseItems = purchaseHistoryQuery.Count(); // Số lượng mục mua hàng tổng cộng
                    var totalPages = (int)Math.Ceiling((double)totalPurchaseItems / pageSize); // Tính tổng số trang

                    // Truy vấn dữ liệu mua hàng đã phân trang
                    var pagedList = purchaseHistoryQuery
                        .Skip((page - 1) * pageSize) // Bỏ qua các mục trên trang trước
                        .Take(pageSize) // Chọn số lượng mục cho trang hiện tại
                        .ToList();

                    // Gán danh sách mua hàng đã phân trang vào model
                    profileModel.PurchaseHistory = new StaticPagedList<PurchaseHistoryVM>(pagedList, page, pageSize, totalPurchaseItems);
                    //return RedirectToAction("Profile", new { page = page, pageSize = pageSize });
                    return View(profileModel);
                }
                else
                {
                    TempData["Message"] = "Customer not found";
                    return View();
                }
            }
            return View();
        }


     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(DatLaiMatKhauVM model, IFormFile newImage)
        {
            //if(ModelState.IsValid)
            // {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == MySettings.CLAIM_CUSTOMER_ID);
            if (userIdClaim != null)
            {
                var userId = userIdClaim.Value;
                var khachHang = db.KhachHangs.Find(userId);

                if (khachHang != null)
                {
                    khachHang.HoTen = model.FullName;
                    khachHang.Email = model.Email;
                    khachHang.DiaChi = model.Address;
                    khachHang.DienThoai = model.PhoneNumber;

                    if (newImage != null && newImage.Length > 0)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(newImage, "KhachHang");
                    }


                    db.Update(khachHang);
                    await db.SaveChangesAsync();

                    return RedirectToAction("Profile");
                }
                else
                {
                    TempData["Message"] = "Customer not found";
                }
            }
            // }
            else
            {
                TempData["Message"] = "User not authenticated";
            }
            return View("Profile", model);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(DatLaiMatKhauVM model)
        {
            var userId = HttpContext.User.FindFirstValue(MySettings.CLAIM_CUSTOMER_ID);

            var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKh == userId);

            if (khachHang == null)
            {
                return NotFound();
            }

            string hashedOldPassword = model.OldPassword.ToMd5Hash(khachHang.RandomKey);
            if (hashedOldPassword != khachHang.MatKhau)
            {
                ModelState.AddModelError("Error", "Old password is not correct.");
                return View("Profile", model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("Error", "New password and confirm password not match.");
                return View("Profile", model);
            }

            TempData["Message"] = "Changed password successful.";
            khachHang.MatKhau = model.NewPassword.ToMd5Hash(khachHang.RandomKey);
            db.SaveChanges();

            return RedirectToAction("Profile");
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

                await _myEmailSender.SendEmailAsync(khachHang.Email, "Đặt lại mật khẩu", $"Please <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>click here</a> to reset your password.");


                return RedirectToAction("NotificationRSPassword");
            }
            else
            {
                ModelState.AddModelError("Error", "MaKh does not exist.");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> NotificationRSPassword()
        {
            return View();
        }


        [HttpGet]
        public IActionResult DatLaiMatKhau()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DatLaiMatKhau(ResetPasswordVM model)
        {
            var userId = TempData["UserId"]?.ToString();
            var token = TempData["Token"]?.ToString();

            model.UserId = userId;
            model.Token = token;

            if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Token))
            {
                return RedirectToAction("Error", "Home");
            }

            var khachHang = db.KhachHangs.SingleOrDefault(k => k.MaKh == model.UserId && k.ResetToken == model.Token);

            if (khachHang != null)
            {
                TempData["Message"] = "Changed password successful.";
                khachHang.MatKhau = model.NewPassword.ToMd5Hash(khachHang.RandomKey);
                khachHang.ResetToken = null;
                db.SaveChanges();

                return RedirectToAction("DatLaiMatKhauThanhCong");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpGet]
        public async Task<IActionResult> DatLaiMatKhauThanhCong()
        {
            return View();
        }
        #endregion

    }
}
