using Microsoft.Extensions.Configuration;
using MoneyMind_BLL.Services.Interfaces;
using System.Net.Mail;
using System.Net;
using System.Text;
using MoneyMind_BLL.DTOs.Emails;

namespace MoneyMind_BLL.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendConfirmEmailAsync(string toEmail, string subject, string templatePath, EmailConfirmRequest emailConfirmRequest)
        {
            // Đọc file template từ thư mục hiện tại
            var templateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templatePath);

            if (!File.Exists(templateFilePath))
            {
                throw new FileNotFoundException("Email template not found.", templateFilePath);
            }

            var body = await File.ReadAllTextAsync(templateFilePath);

            // Replace placeholders in the template with actual data
            body = body.Replace("@Model.Username", emailConfirmRequest.Username)
                       .Replace("@Model.ConfirmationLink", emailConfirmRequest.ConfirmationLink);

            // SMTP settings
            var smtpHost = configuration["Email:Smtp:Host"];
            var smtpPort = int.Parse(configuration["Email:Smtp:Port"]);
            var smtpUser = configuration["Email:Smtp:User"];
            var smtpPass = configuration["Email:Smtp:Password"];
            var fromEmail = configuration["Email:Smtp:FromEmail"];

            // Kiểm tra các cấu hình SMTP
            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass) || string.IsNullOrEmpty(fromEmail))
            {
                throw new InvalidOperationException("SMTP settings are not properly configured.");
            }

            // Create email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
            };

            mailMessage.To.Add(toEmail);

            // Tạo một đối tượng AlternateView cho HTML content
            var htmlView = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, "text/html");

            // Thêm hình ảnh (embedded image)
            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logos", "Logo_MoneyMind-RemoveBg.png"); // Đường dẫn đến hình ảnh
            if (File.Exists(imagePath))
            {
                var logo = new LinkedResource(imagePath, "image/png")
                {
                    ContentId = "Logo_MoneyMind",
                    TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                };
                htmlView.LinkedResources.Add(logo);
            }
            else
            {
                throw new FileNotFoundException("Embedded image not found.", imagePath);
            }

            mailMessage.AlternateViews.Add(htmlView);

            // Send email
            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email.", ex);
            }
        }
    }
}
