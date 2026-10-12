using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace WebLab.Services
{
    public class EmailSenderService : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            var senderName = _configuration["EmailSender:SenderName"] ?? "WebLab";
            var senderEmail = _configuration["EmailSender:SenderEmail"] ?? "toncukartem@gmail.com";
            var password = _configuration["EmailSender:Password"] ?? "password";
            var host = _configuration["EmailSender:Host"] ?? "smtp.gmail.com";
            var port = int.Parse(_configuration["EmailSender:Port"] ?? "587");

            emailMessage.From.Add(new MailboxAddress(senderName, senderEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.Auto);
                    await client.AuthenticateAsync(senderEmail, password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch
            {
            }
        }
    }
}
