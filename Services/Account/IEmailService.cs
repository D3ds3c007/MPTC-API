using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Education;
using MimeKit;
using MailKit.Net.Smtp;



namespace MPTC_API.Services.Authentication
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string messageBody);
        Task SendPasswordResetEmail(string toEmail, string resetLink);
        Task SendWelcomeEmail(string toEmail, string password);
        
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

        // The updated SendEmailAsync method with dynamic subject and message body
        public async Task SendEmailAsync(string toEmail, string subject, string messageBody)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var email = smtpSettings["Username"];
            var password = smtpSettings["Password"];
            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"]);

            // Construct the email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MPTC Security System", email));
            message.To.Add(new MailboxAddress("Recipient Name", toEmail));
            message.Subject = subject;  // Use the dynamic subject

            // Set the message body as dynamic HTML content
            message.Body = new TextPart("html")
            {
                Text = messageBody  // Use the dynamic message body
            };

            // Send the email using SMTP
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(email, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public  async Task SendPasswordResetEmail(string toEmail, string resetLink)
        {
            string subject = "Reset your password";
            string messageBody = $"<p>Please click the link to reset your password:</p><p><a href='{resetLink}'>Reset Password</a></p>";

            await SendEmailAsync(toEmail, subject, messageBody);
        }

        public async Task SendWelcomeEmail(string toEmail, string password)
        {
            string subject = "Welcome to MPTC!";
            string messageBody = $"<p>Your account has been created successfully. Your password is: {password}</p>";

            await SendEmailAsync(toEmail, subject, messageBody);
        }
    }

    
}

    


