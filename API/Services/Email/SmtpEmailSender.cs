using System.Net;
using System.Net.Mail;

namespace API.Services.Email
{
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
        {
            var host = _configuration["Email:Smtp:Host"];
            var portValue = _configuration["Email:Smtp:Port"];
            var username = _configuration["Email:Smtp:Username"];
            var password = _configuration["Email:Smtp:Password"];
            var from = _configuration["Email:From"];
            var displayName = _configuration["Email:DisplayName"] ?? "NIVO";

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(portValue) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(from))
                throw new InvalidOperationException("Chưa cấu hình Email:Smtp và Email:From trong appsettings hoặc User Secrets.");

            if (!int.TryParse(portValue, out var port))
                throw new InvalidOperationException("Email:Smtp:Port không hợp lệ.");

            using var message = new MailMessage
            {
                From = new MailAddress(from, displayName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(to));

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = _configuration.GetValue("Email:Smtp:EnableSsl", true),
                Credentials = new NetworkCredential(username, password)
            };

            cancellationToken.ThrowIfCancellationRequested();
            await client.SendMailAsync(message);
        }
    }
}
