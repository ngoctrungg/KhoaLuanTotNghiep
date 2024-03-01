using AutoMapper;
using KLTN_E.Data;
using KLTN_E.Helpers;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KLTN_E.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly KltnContext db;
        private readonly IMapper _mapper;

        public KhachHangController(KltnContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(RegisterVM model, IFormFile Hinh)
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
                    return RedirectToAction("Index", "HangHoa");
                }catch (Exception ex)
                {

                }
            }
            return View();
        }
    }
}
