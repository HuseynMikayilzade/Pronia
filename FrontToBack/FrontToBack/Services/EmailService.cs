using FrontToBack.Interfaces;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Net;
using System.Net.Mail;

namespace FrontToBack.Services
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string ToEmail,string subject , string body , bool ishtml)
        {
            SmtpClient smtpClient = new SmtpClient(_configuration["Email:Host"], Convert.ToInt32(_configuration["Email:Port"]));
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(_configuration["Email:LoginEmail"], _configuration["Email:Password"]);

            MailAddress from = new MailAddress(_configuration["Email:LoginEmail"],"Pronia");
            MailAddress to = new MailAddress(ToEmail);
            MailMessage  message = new MailMessage(from,to);
            message.Subject =subject;
            message.Body = body;
            message.IsBodyHtml = ishtml;
            await smtpClient.SendMailAsync(message);
        }
    }
}
