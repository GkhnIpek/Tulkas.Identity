using System.Net;
using System.Net.Mail;

namespace Tulkas.Identity.WebApi.Helper
{
    public static class EmailConfirmation
    {
        public static void SendEmail(string link, string email)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com") { Port = 587, EnableSsl = true, UseDefaultCredentials = true};

            mail.From = new MailAddress("rackledev@gmail.com");
            mail.To.Add("gokipek91@gmail.com");

            mail.Subject = $"www.tulkas.identity.com::Email Doğrulama";
            mail.Body = $"<h2>Email adresinizi doğrulamak için lütfen aşağıdaki linke tıklayınız.</h2><hr/>";
            mail.Body += $"<a href='{link}'> email doğrulama linki</a>";
            mail.IsBodyHtml = true;
            smtpClient.Credentials = new NetworkCredential("rackledev@gmail.com", "RackleDev2020?");

            smtpClient.Send(mail);
        }
    }
}
