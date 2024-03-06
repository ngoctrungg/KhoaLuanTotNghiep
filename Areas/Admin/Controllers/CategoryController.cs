using KLTN_E.Data;
using Microsoft.AspNetCore.Mvc;

namespace KLTN_E.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly KltnContext db;

        public CategoryController(KltnContext context) 
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
