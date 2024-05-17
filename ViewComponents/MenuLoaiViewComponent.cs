using KLTN_E.Data;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KLTN_E.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly KltnContext db;

        public MenuLoaiViewComponent(KltnContext context) => db = context;
        
        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(l => new MenuLoaiVM
            {
                MaLoai = l.MaLoai,
                TenLoai = l.TenLoai,
                SoLuong = l.HangHoas.Count
            }).OrderBy(p => p.TenLoai);
            return View(data);
        }
    }
}
