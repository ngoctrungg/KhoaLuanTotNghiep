using KLTN_E.ViewModels;

namespace KLTN_E.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExcuse(IQueryCollection collections);

    }
}
