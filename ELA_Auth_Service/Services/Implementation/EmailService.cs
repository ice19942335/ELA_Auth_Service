using System.Net;
using System.Threading.Tasks;
using ELA_Auth_Service.Domain.DTO;
using ELA_Auth_Service.Options;
using ELA_Auth_Service.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ELA_Auth_Service.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly SendGridSettings _sendGridSettings;

        public EmailService(SendGridSettings sendGridSettings) => _sendGridSettings = sendGridSettings;

        public async Task<PasswordUpdateDto> SendEmailAsync(string email, string subject, string message)
        {
            var client = new SendGridClient(_sendGridSettings.Key);

            var msg = new SendGridMessage
            {
                From = new EmailAddress(_sendGridSettings.FromEmail),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            var sendMsgResult = await client.SendEmailAsync(msg);

            if (sendMsgResult.StatusCode != HttpStatusCode.Accepted)
                return new PasswordUpdateDto { Errors = new[] { $"Status code: {sendMsgResult.StatusCode}" }, Success = false };

            return new PasswordUpdateDto { Success = true };
        }
    }
}