using KLTN_E.Data;
using KLTN_E.Models;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KLTN_E.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly KltnContext db;

        public HomeController(ILogger<HomeController> logger, KltnContext context)
		{
			_logger = logger;
			db = context;
		}

		public IActionResult Index()
		{
            var hangHoas = db.HangHoas.AsQueryable();

            
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHH = p.MaHh,
                TenHH = p.TenHh,
                Hinh = p.Hinh ?? "",
                DonGia = p.DonGia ?? 0,
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            return View(result);
		}

		[Route("/404")]
        public IActionResult NotFound404()
        {
            return View();
        }

        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
