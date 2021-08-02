using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public class SendGridEmailService : IEmailService
    {
        private SendGridClient _sendGridClient;
        private string _simucraftEmail;

        public SendGridEmailService(SendGridClient sendGridClient, string simucraftEmail)
        {
            _sendGridClient = sendGridClient;
            _simucraftEmail = simucraftEmail;
        }

        public async Task EmailAsync(string subject, string body, string to)
        {
            var message = new SendGridMessage();
            message.SetFrom(new EmailAddress(_simucraftEmail, "Simucraft"));
            message.AddTo(new EmailAddress(to));
            message.SetSubject(subject);
            message.AddContent(MimeType.Html, body);

            var response = await _sendGridClient.SendEmailAsync(message);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                throw new InvalidOperationException("Email failed to send.");
        }
    }
}
