using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace LeaderPush.Controllers
{
    public class SupportController : Controller
    {
        // GET: Support
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Send(string Name, string Email, string Text) {


            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;
            bool enableSSL = true;

            string emailFrom = "tuncayhuseynov@gmail.com";
            string password = "5591980supertun";
            string emailTo = "berkay2828@gmail.com";


            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = "LeaderPush Support Desk";
                mail.Body = "Name: " + Name + "\nEmail: " + Email + "\nMessage: " + Text;
                mail.IsBodyHtml = false;
                // Can set to false, if you are sending pure text.



                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }

            return View();
        }
    }
}