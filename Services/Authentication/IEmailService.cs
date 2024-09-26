using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Education;
using MimeKit;
using MailKit.Net.Smtp;



namespace MPTC_API.Services.Authentication
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string token);
    }
}
namespace MPTC_API.Services.Authentication
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration; 
        }
        public async Task SendEmailAsync(string toEmail, string resetLink)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var email = smtpSettings["Username"];
            var password = smtpSettings["Password"];
            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"]);

             var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MPTC Security System", email));
            message.To.Add(new MailboxAddress("Recipient Name", toEmail));
            message.Subject = "Your reset password URL";

            message.Body = new TextPart("html")
            {
                Text = $"<p>Please click the link to verify your account:</p><p><a href='{resetLink}'>Reset Password</a></p>"
            };

             // Send the email
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(email, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

        
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            throw new NotImplementedException();
        }
    }
    
}

    


