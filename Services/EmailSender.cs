using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace ReservationApp.Services;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var mail = "serverguard@zohomail.eu";
        
        var client = new SmtpClient("smtp.zoho.eu", 587)
        {
            Credentials = new NetworkCredential(mail, pw),
            EnableSsl = true
        };
        return client.SendMailAsync(new MailMessage(from: mail,
                                                    to: email,
                                                    subject,htmlMessage
                                                    ));
    }
}
