using KLTN_E.Data;
using KLTN_E.ViewModels;

namespace KLTN_E.Services
{
    public interface IPurchaseHistoryService
    {
        public List<PurchaseHistoryVM> GetPurchaseHistory(string maKhachHang);
    }
}
