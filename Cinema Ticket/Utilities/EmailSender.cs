using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Ecommerce531.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    "ahmedsaber19921081@gmail.com",
                    "nbvv shay hnkp schl"
                )
            };

            return client.SendMailAsync(
                new MailMessage(
                    from: "ahmedsaber19921081@gmail.com",
                    to: email,
                    subject,
                    htmlMessage
                )
                {
                    IsBodyHtml = true
                });
        }
    }
}