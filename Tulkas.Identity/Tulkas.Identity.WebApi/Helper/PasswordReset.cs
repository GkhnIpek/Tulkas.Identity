using System.Net;
using System.Net.Mail;

namespace Tulkas.Identity.WebApi.Helper
{
    public static class PasswordReset
    {
        public static void PasswordResetSendEmail(string link, string email)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com") {Port = 587, EnableSsl = true};

            mail.From = new MailAddress("gokipek91@gmail.com");
            mail.To.Add(email);

            mail.Subject = $"www.tulkas.identity.com::Şifre Sıfırlama";
            mail.Body = $"<h2>Şifrenizi yenilemek için lütfen aşağıdaki linke tıklayınız.</h2><hr/>";
            mail.Body += $"<a href='{link}'> şifre yenileme linki</a>";
            mail.IsBodyHtml = true;
            smtpClient.Credentials = new NetworkCredential("gokipek91@gmail.com", "******");

            smtpClient.Send(mail);
        }
    }
}
