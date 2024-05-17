using Microsoft.AspNetCore.Mvc;

namespace KLTN_E.Areas.Admin.Controllers
{
    public class DashboardsController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
