using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace KLTN_E.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (MailMessage mm = new MailMessage("groscul202@gmail.com", email))
            {
                mm.Subject = subject;
                mm.Body = htmlMessage;
                mm.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
                {
                    smtp.Timeout = 10000; // Tăng thời gian chờ lên 10 giây (mặc định là 100000 ms)
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("groscul202@gmail.com", "ixzy zepe dlii sayn");

                    await smtp.SendMailAsync(mm);
                }
            }
        }
    }

}
