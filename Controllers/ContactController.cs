using KLTN_E.Services;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using KLTN_E.Data;

namespace KLTN_E.Controllers
{
    public class ContactController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly IMyEmailSender _myEmailSender;
        private readonly KltnContext db;
        private readonly IConfiguration _configuration;

        public ContactController(IEmailSender emailSender, IMyEmailSender myEmailSender, KltnContext context, IConfiguration configuration)
        {
            _emailSender = emailSender;
            _myEmailSender = myEmailSender;
            db = context;
            _configuration = configuration;
        }

        [Route("/contact")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [Route("/contact/send-message")]
        [HttpPost]
        public IActionResult SendMessage(ContactViewModel model)
        {
            SendEmail(model.Email, model.Subject, model.Message);
           

            return RedirectToAction("ContactSuccess");
        }

        public void SendEmail(string email, string subject, string htmlMessage)
        {
            var customerEmail = email;

            using (MailMessage mm = new MailMessage())
            {
                mm.From = new MailAddress(_configuration["NetMail:sender"]);

                mm.To.Add(new MailAddress(_configuration["AdminEmail"]));

                mm.Subject = subject;
                mm.Body = $"Customer's Email: {customerEmail}<br />{htmlMessage}";
                mm.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = _configuration["NetMail:SmtpHost"];
                smtp.EnableSsl = true;
                NetworkCredential networkCred = new NetworkCredential(_configuration["NetMail:sender"], _configuration["NetMail:senderpassword"]);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = networkCred;
                smtp.Port = 587;

                smtp.Send(mm);
            }
        }

        [Route("/contact/send-message-success")]
        public IActionResult ContactSuccess()
        {
            return View();
        }


    }
}
