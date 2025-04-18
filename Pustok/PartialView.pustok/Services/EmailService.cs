using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using PartialView.pustok.Settings;

namespace PartialView.pustok.Services
{
    public class EmailService
    {
        public void SendEmail(string to , string subject , string body,string from,string appPassw,EmailSetting emailSetting)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(emailSetting.SmtpServer, emailSetting.SmtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(from, appPassw);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

    }
}
