using KLTN_E.Data;
using KLTN_E.Helpers;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System.Dynamic;
using System.Net.Http;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace KLTN_E.Controllers
{
    public class GGLoginController : Controller
    {
        private readonly KltnContext db;
        private readonly IConfiguration _configuration;

        public GGLoginController(KltnContext context, IConfiguration configuration)
        {
            db = context;
            _configuration = configuration;
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
                    var newUser = new KhachHang
                    {
                        MaKh = email,
                        Email = email,
                        HoTen = fullName,
                        Hinh = "",
                        HieuLuc = true,
                        RandomKey = MyUtil.GenerateRandomKey(),
                        VaiTro = 0

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



        #region facebook login
        public IActionResult LoginWithFacebook()
        {
            var appId = _configuration.GetValue<string>("FacebookKeys:AppId");
            var redirectUri = "https://localhost:7121/GGLogin/FacebookCallback";
            var facebookLoginUrl = $"https://www.facebook.com/v12.0/dialog/oauth?client_id={appId}&redirect_uri={redirectUri}&response_type=code&scope=email";
            return Redirect(facebookLoginUrl);
        }

        [HttpGet]
        public async Task<IActionResult> FacebookCallback(string code)
        {
            var appId = _configuration.GetValue<string>("FacebookKeys:AppId");
            var appSecret = _configuration.GetValue<string>("FacebookKeys:AppSecret");
            var redirectUri = "https://localhost:7121/GGLogin/FacebookCallback";

            // Gửi yêu cầu để đổi mã code thành access token
            var accessTokenResponse = await ExchangeCodeForAccessToken(appId, appSecret, redirectUri, code);
            if (accessTokenResponse != null && accessTokenResponse.ContainsKey("access_token"))
            {
                var accessToken = accessTokenResponse["access_token"];

                // Gửi yêu cầu để lấy thông tin người dùng từ Facebook bằng access token
                var userInfoResponse = await GetUserInfoFromFacebook(accessToken);
                if (userInfoResponse != null)
                {
                    // Xử lý thông tin người dùng
                    var email = userInfoResponse.email;
                    var fullName = userInfoResponse.name;
                    //var profileImage = userInfoResponse.picture.data.url;
                    var profileImageUrl = userInfoResponse.picture.data.url.ToString();
                    var imageName = await MyUtil.UploadHinhFromUrl(profileImageUrl, "KhachHang");
                    if (email != null)
                    {
                        string emailString = null;

                        if (email is string stringValue)
                        {
                            emailString = stringValue;
                        }
                        else if (email.GetType().GetProperty("Value") != null)
                        {
                            var valueProperty = email.GetType().GetProperty("Value");
                            var value = valueProperty.GetValue(email);
                            if (value != null)
                            {
                                emailString = value.ToString();
                            }
                        }
                        // Kiểm tra xem người dùng đã tồn tại trong hệ thống hay chưa
                        var existingUser = await db.KhachHangs.FirstOrDefaultAsync(u => u.Email == emailString);
                        if (existingUser == null)
                        {
                            var hinh = await MyUtil.UploadHinhFromUrl(profileImageUrl, "KhachHang");
                            var newUser = new KhachHang
                            {
                                MaKh = emailString,
                                Email = emailString,
                                HoTen = fullName,
                                Hinh = hinh,
                                HieuLuc = true,
                                RandomKey = MyUtil.GenerateRandomKey(),
                                VaiTro = 0
                            };
                            db.Add(newUser);
                            await db.SaveChangesAsync();

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
                }
            }

            // Xử lý trường hợp không thành công
            // Chuyển hướng người dùng đến trang đăng nhập hoặc trang lỗi
            return RedirectToAction("DangNhap", "Account");
        }


        private async Task<Dictionary<string, string>> ExchangeCodeForAccessToken(string appId, string appSecret, string redirectUri, string code)
        {
            var client = new HttpClient();
            var url = $"https://graph.facebook.com/v12.0/oauth/access_token?client_id={appId}&client_secret={appSecret}&redirect_uri={redirectUri}&code={code}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            }
            return null;
        }

        // Phương thức để gửi yêu cầu để lấy thông tin người dùng từ Facebook bằng access token
        private async Task<dynamic> GetUserInfoFromFacebook(string accessToken)
        {
            var client = new HttpClient();
            var url = $"https://graph.facebook.com/me?fields=id,email,name,picture&access_token={accessToken}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<dynamic>(content);
            }
            return null;
        }


        #endregion




    }
}
