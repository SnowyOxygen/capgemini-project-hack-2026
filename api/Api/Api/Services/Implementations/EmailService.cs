using Api.Services.Interfaces;

namespace Api.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            // TODO: Implement actual email sending using SendGrid, SMTP, or other email service
            // For now, we'll just log the reset token (in production, this should send an email)
            
            var resetUrl = $"{_configuration["Frontend:Url"]}/reset-password?token={resetToken}";
            
            _logger.LogInformation(
                "Password reset requested for {Email}. Reset URL: {ResetUrl}", 
                toEmail, 
                resetUrl);

            // Simulate async email sending
            await Task.Delay(100);

            // In production, replace the above with actual email sending logic:
            /*
            using var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(
                    _configuration["Smtp:Username"],
                    _configuration["Smtp:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:From"]),
                Subject = "Password Reset Request",
                Body = $"Click the following link to reset your password: {resetUrl}",
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
            */
        }
    }
}
